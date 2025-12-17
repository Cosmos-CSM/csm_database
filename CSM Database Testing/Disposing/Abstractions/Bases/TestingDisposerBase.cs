using System.Collections.Concurrent;

using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Database_Testing.Disposing.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CSM_Database_Testing.Disposing.Abstractions.Bases;

/// <summary>
///     Database context factory delegate.
/// </summary>
/// <returns>
///     Database context.
/// </returns>
public delegate DbContext DatabaseFactory();


/// <inheritdoc cref="ITestingDisposer"/>
public abstract class TestingDisposerBase
    : ITestingDisposer {

    /// <summary>
    ///     Entities databases context factories.  
    /// </summary>
    protected readonly Dictionary<Type, DatabaseFactory> _dbFactories = [];

    /// <summary>
    ///     Current disposition queue order and context.
    /// </summary>
    protected readonly ConcurrentDictionary<Type, IEntity[]> _queue = [];

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="dbFactories">
    ///     Database contexts used during testing data creation for disposition.
    /// </param>
    public TestingDisposerBase(params DatabaseFactory[] dbFactories) {
        foreach (DatabaseFactory factory in dbFactories) {
            using DbContext instance = factory();

            Type dbType = instance.GetType();
            _dbFactories.Add(dbType, factory);
            _queue.AddOrUpdate(
                    dbType,
                    (_) => [],
                    (_, prev) => [.. prev]
                );
        }
    }

    public void Push(IEntity entity) {
        if (_dbFactories.ContainsKey(entity.Database)) {
            _queue.AddOrUpdate(
                    entity.Database,
                    (_) => [entity],
                    (_, prev) => [.. prev, entity]
                );

        } else {
            throw new Exception($"Tried to push a record for Disposition with no subscribed database owning factory ({entity.Database.Name}).");
        }
    }

    public void Push(IEntity[] entities) {
        foreach (IEntity Record in entities) {
            Push(Record);
        }
    }

    public void Dispose() {
        foreach (KeyValuePair<Type, IEntity[]> Database in _queue) {
            Type dbType = Database.Key;
            DatabaseFactory factory = _dbFactories[dbType];

            using DbContext database = factory();
            IEnumerable<IEntity> committedEntities = Database.Value.Where(i => i.Id > 0).Reverse();

            foreach (IEntity committedEntity in committedEntities) {
                EntityEntry entry = database.Entry(committedEntity);
                if (entry.GetDatabaseValues() is null) {
                    continue;
                }

                // Delete ICollection Entities before deleting the main entity.
                foreach (var property in committedEntity.GetType().GetProperties()) {
                    if (typeof(IEnumerable<IEntity>).IsAssignableFrom(property.PropertyType)) {
                        if (property.GetValue(committedEntity) is IEnumerable<IEntity> collection) {
                            foreach (var item in collection) {
                                EntityEntry subEntry = database.Entry(item);
                                if (subEntry.GetDatabaseValues() is null) {
                                    continue;
                                }

                                subEntry.State = EntityState.Deleted;
                            }
                        }
                    }
                }


                entry.DetectChanges();
                entry.State = EntityState.Deleted;
                database.SaveChanges();
            }
        }
    }
}
