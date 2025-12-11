using System.Data;
using System.Reflection;

using CSM_Database_Core.Core.Errors;
using CSM_Database_Core.Core.Extensions;
using CSM_Database_Core.Core.Utils;
using CSM_Database_Core.Depots.Abstractions.Interfaces;
using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Bases;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CSM_Database_Core.Depots.Abstractions.Bases;

/// <inheritdoc cref="IDepot{TEntity}"/>
/// <typeparam name="TDatabase">
///     Database context owner.
/// </typeparam>
/// <typeparam name="TEntity">
///     Type of the depot entity handled.
/// </typeparam>>
public abstract class DepotBase<TDatabase, TEntity>
    : IDepot<TEntity>
    where TDatabase : DatabaseBase<TDatabase>
    where TEntity : class, IEntity, new() {

    /// <summary>
    ///     System data disposition manager.
    /// </summary>
    protected readonly IDisposer<IEntity>? _disposer;

    /// <summary>
    ///     Name to handle direct transactions (not-attached)
    /// </summary>
    protected readonly TDatabase _db;

    /// <summary>
    ///     DBSet handler into <see cref="_db"/> to handle fastlike transactions related to the <see cref="TEntity"/> 
    /// </summary>
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    ///     Generates a new instance of a <see cref="DepotBase{TMigrationDatabases, TMigrationSet}"/> base.
    /// </summary>
    /// <param name="Database">
    ///     The <typeparamref name="TDatabase"/> that stores and handles the transactions for this <see cref="TEntity"/> concept.
    /// </param>
    public DepotBase(TDatabase Database, IDisposer<IEntity>? Disposer) {
        _db = Database;
        _disposer = Disposer;
        _dbSet = Database.Set<TEntity>();
    }

    #region (Private / Protected) Functions / Methods


    /// <summary>
    /// Stores the specified entity and its nested entities in the database.
    /// </summary>
    /// <remarks>This method processes the specified entity and its nested entities, adding them to the
    /// database. The method ensures that nested entities are stored in the correct order to maintain referential integrity.</remarks>
    /// <param name="common">The root entity to be stored. Nested entities within this entity will also be processed and stored.</param>
    /// <param name="save">A boolean value indicating whether to immediately save changes to the database. <see langword="true"/> to save
    /// changes after storing the entities; otherwise, <see langword="false"/>.</param>
    public async Task<TEntity> Store(TEntity root, bool save = false) {
        HashSet<IEntity> entitiesToAdd = [];

        StoreNestedEntities(root, entitiesToAdd);

        foreach (IEntity entity in entitiesToAdd.Reverse()) {
            if (entity.Id == 0) {
                _db.Add(entity);
                _disposer?.Push(entity);
            }
        }

        if (save) await _db.SaveChangesAsync();

        return root;
    }

    /// <summary>
    /// Recurses through the nested entities of a common entity and stores them in a hash set to avoid duplicates.
    /// </summary>
    /// <param name="entity">Current entity to process and store.</param>
    /// <param name="entitiesHash">List of stored entities. The content is verified to avoid duplications. </param>
    static void StoreNestedEntities(IEntity entity, HashSet<IEntity> entitiesHash) {

        if (entity == null || entitiesHash.Contains(entity)) return;
        entitiesHash.Add(entity);

        Type type = entity.GetType();
        foreach (PropertyInfo prop in type.GetProperties()) {
            var value = prop.GetValue(entity);

            if (value is IEntity nestedEntity) {
                StoreNestedEntities(nestedEntity, entitiesHash);
            } else if (value is IEnumerable<IEntity> collection) {
                foreach (var item in collection) {
                    StoreNestedEntities(item, entitiesHash);
                }
            }
        }

    }

    protected TEntity2 ValidateDependency<TEntity2>(TEntity2 dependencyEntity)
        where TEntity2 : class, IEntity, new() {

        TEntity2? tmpDependency = dependencyEntity;
        tmpDependency = tmpDependency.Id > 0
            ? _db.Set<TEntity2>().Where(dep => dep.Id == tmpDependency.Id).FirstOrDefault()
            : throw new Exception($"Dependencies aren't allowed to be auto-created on main Entity creation, you need to create the Dependency first in its corresponding [Depot]");

        return tmpDependency is null
            ? throw new Exception($"[{GetType().Name}] entity requires [{typeof(TEntity2)}] dependency")
            : tmpDependency;
    }

    #endregion

    #region View 

    public async Task<ViewOutput<TEntity>> View(QueryInput<TEntity, ViewInput<TEntity>> input) {
        ViewInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> processedQuery = _dbSet.Process(
                input,
                (query) => {
                    processedQuery = query.OrderView(parameters.Orderings);
                    processedQuery = processedQuery.FilterView(parameters.Filters);

                    return processedQuery;
                }
            );


        PaginationOutput<TEntity> paginationOutput = await processedQuery.PaginateView(parameters.Page, parameters.Range, parameters.Export);

        return new ViewOutput<TEntity>() {
            Page = parameters.Page,
            Pages = paginationOutput.PagesCount,
            Count = paginationOutput.EntitiesCount,
            Entities = [.. paginationOutput.Query],
        };
    }

    #endregion

    #region Create

    /// <summary>
    ///     Creates a new overwritten into the dataDatabases.
    /// </summary>
    /// <param name="entity">
    ///     <see cref="TEntity"/> to store.
    /// </param>
    /// <returns> 
    ///     The stored object. (Object Id is always auto-generated)
    /// </returns>
    public virtual async Task<TEntity> Create(TEntity entity) {
        entity.Timestamp = DateTime.UtcNow;
        entity.EvaluateWrite();

        entity = DatabaseUtils.SanitizeEntity(_db, entity);
        await _dbSet.AddAsync(entity);

        _disposer?.Push(entity);
        await _db.SaveChangesAsync();

        return entity;
    }


    /// <summary>
    ///     Creates a collection of records into the dataDatabases. 
    ///     <br>
    ///         Depending on <paramref name="sync"/> the transaction performs different,
    ///         the operation iterates the desire collection to store and collects all the 
    ///         failures gathered during the operation.
    ///     </br>
    /// </summary>
    /// <param name="entities">
    ///     The collection to store.
    /// </param>
    /// <param name="sync">
    ///     Determines if the transaction should be broken at the first failure catched. This means that
    ///     the previous successfully stored objects will be kept as stored but the next ones objects desired
    ///     to be stored won't continue, the operation will throw new exception.
    /// </param>
    /// <returns>
    ///     A <see cref="EntityBatchOut{TSet}"/> that stores a collection of failures, and successes caught.
    /// </returns>
    public virtual async Task<BatchOperationOutput<TEntity>> Create(ICollection<TEntity> entities, bool sync = false) {
        TEntity[] attached = [];
        EntityError<TEntity>[] failures = [];

        foreach (TEntity entity in entities) {
            try {
                TEntity attachedEntity = await Create(entity);
                attached = [.. attached, attachedEntity];
            } catch (Exception excep) {
                if (sync) {
                    throw;
                }

                EntityError<TEntity> fail = new(EntityErrorEvents.CREATE_FAILED, entity, excep);
                failures = [.. failures, fail];
            }
        }
        _db.SaveChanges();
        return new(attached, failures);
    }

    #endregion

    #region Read

    /// <summary>
    ///     Reads into the <see cref="TEntity"/> database [Entity] for matched records.
    /// </summary>
    /// <param name="id">
    ///     Identifier of the desired <typeparamref name="TEntity"/>.
    /// </param>
    /// <returns> <see cref="TEntity"/> insatcne found </returns>
    /// <exeption cref="XDepot">
    ///     Thrown when the <see cref="TEntity"/> couldn't be found.
    /// </exeption>
    public async Task<TEntity> Read(long id) {
        TEntity? entity = await _dbSet.Where(
                e => e.Id == id
            )
            .FirstOrDefaultAsync()
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{nameof(IEntity.Id)} = {id}");

        entity.EvaluateRead();
        return entity;
    }

    public async Task<BatchOperationOutput<TEntity>> Read(long[] ids) {

        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];
        foreach (long id in ids) {

            try {
                TEntity success = await Read(id);
                successes.Add(success);
            } catch (Exception ex) {
                failures.Add(
                        new EntityError<TEntity>(
                                EntityErrorEvents.READ_FAILED,
                                new TEntity {
                                    Id = id
                                },
                                ex
                            )
                    );
            }
        }

        return new BatchOperationOutput<TEntity>([.. successes], [.. failures]);
    }

    public async Task<BatchOperationOutput<TEntity>> Read(QueryInput<TEntity, FilterQueryInput<TEntity>> input) {
        FilterQueryInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> processedQuery = _dbSet.Process(
                input,
                sourceQuery => {
                    sourceQuery = _dbSet.Where(parameters.Filter);
                    return sourceQuery;
                }
            );

        if (!processedQuery.Any()) {
            return new BatchOperationOutput<TEntity>([], []);
        }

        TEntity[] resultItems = parameters.Behavior switch {
            FilteringBehaviors.First => [await processedQuery.FirstAsync()],
            FilteringBehaviors.Last => [await processedQuery.Order().LastAsync()],
            FilteringBehaviors.All => await processedQuery.ToArrayAsync(),
            _ => throw new NotImplementedException(),
        };

        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];
        foreach (TEntity item in resultItems) {
            try {
                item.EvaluateRead();
                successes.Add(item);
            } catch (Exception exception) {
                EntityError<TEntity> failure = new(EntityErrorEvents.READ_VALIDATION_FAILED, item, exception);
                failures.Add(failure);
            }
        }

        if (parameters.Behavior == FilteringBehaviors.First && failures.Count > 0) {
            throw failures[0];
        }

        return new BatchOperationOutput<TEntity>(
                [.. successes],
                [.. failures]
            );
    }

    #endregion

    #region Update 

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"> Lastest data set stored in db sorce. </param>
    /// <param name="overwritten"> Modified set given in update service params. This modifications must be applied to the [current] set in db source. </param>
    void UpdateHelper(IEntity original, IEntity overwritten) {
        EntityEntry previousEntry = _db.Entry(original);
        if (previousEntry.State == EntityState.Unchanged) {
            // Update the non-navigation properties.
            previousEntry.CurrentValues.SetValues(overwritten);
            foreach (NavigationEntry navigation in previousEntry.Navigations) {
                object? newNavigationValue = _db.Entry(overwritten).Navigation(navigation.Metadata.Name).CurrentValue;
                // Validate if navigation is a collection.
                if (navigation.CurrentValue is IEnumerable<object> previousCollection && newNavigationValue is IEnumerable<object> newCollection) {
                    List<object> previousList = [.. previousCollection];
                    List<object> newList = [.. newCollection];
                    // Perform a search for new items to add in the collection.
                    // NOTE: the followings iterations must be performed in diferent code segments to avoid index length conflicts.
                    for (int i = 0; i < newList.Count; i++) {
                        IEntity? newItemSet = (IEntity)newList[i];
                        if (newItemSet != null && newItemSet.Id <= 0) {
                            // Getting the item type to add.
                            Type itemType = newItemSet.GetType();
                            // Getting the Add method from Icollection.
                            MethodInfo? addMethod = previousCollection.GetType().GetMethod("Add", [itemType]);
                            // Adding the new item to Icollection.
                            _ = (addMethod?.Invoke(previousCollection, [newItemSet]));

                        }
                    }
                    // Find items to modify.
                    for (int i = 0; i < previousList.Count; i++) {
                        // For each new item stored in overwritten collection, will search for an ID match and update the overwritten.
                        foreach (object newitem in newList) {
                            if (previousList[i] is IEntity previousItem && newitem is IEntity newItemSet && previousItem.Id == newItemSet.Id) {
                                UpdateHelper(previousItem, newItemSet);
                            }
                        }
                    }
                } else if (navigation.CurrentValue == null && newNavigationValue != null) {
                    // Create a new navigation overwritten.
                    // Also update the attached navigators.
                    //AttachDate(newNavigationValue);
                    EntityEntry newNavigationEntry = _db.Entry(newNavigationValue);
                    newNavigationEntry.State = EntityState.Added;
                    navigation.CurrentValue = newNavigationValue;
                } else if (navigation.CurrentValue != null && newNavigationValue != null) {
                    // Update the existing navigation overwritten
                    if (navigation.CurrentValue is IEntity currentItemSet && newNavigationValue is IEntity newItemSet) {
                        UpdateHelper(currentItemSet, newItemSet);
                    }
                }

            }
        }

    }

    /// <summary>
    ///     Updates the given record calculating the current stored values with the given <paramref name="entity"/> to update and store the new values.
    /// </summary>
    /// <param name="input">
    ///     Operation input parameters.
    /// </param>
    /// <returns></returns>
    /// <remarks>
    ///     Always the record to be overriden will be defined by the <see cref="IEntity.Id"/> property, if isn't given, will try with <see cref="NamedEntityBase.Name"/> property in case the
    ///     [Entity] implementation does have it, otherwise will finally create a new record with the given values.
    /// </remarks>
    /// <exception cref="DepotError{TEntity}">
    ///     <see cref="IDepot{TEntity}"/> related exception.
    /// </exception>
    public async Task<UpdateOutput<TEntity>> Update(QueryInput<TEntity, UpdateInput<TEntity>> input) {
        UpdateInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> processedQuery = _dbSet.Process(
                input,
                (sourceQuery) => sourceQuery
            );

        TEntity entity = parameters.Entity;

        /// --> When the entity is not saved yet.
        if (entity.Id == 0) {
            if (!parameters.Create) {
                throw new DepotError<TEntity>(DepotErrorEvents.CREATE_DISABLED);
            }

            entity = await Create(entity);
            _disposer?.Push(entity);

            return new UpdateOutput<TEntity> {
                Original = null,
                Updated = entity,
            };
        }

        ///
        TEntity? original = await processedQuery
            .Where(obj => obj.Id == entity.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync()
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND);

        if (original == null) {
            if (!parameters.Create)
                throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{typeof(TEntity).Name}.Id = {entity.Id}");

            entity.Id = 0;
            entity = await Create(entity);
            _disposer?.Push(entity);

            return new UpdateOutput<TEntity> {
                Original = null,
                Updated = entity,
            };
        }

        entity = DatabaseUtils.SanitizeEntity(_db, entity);
        _dbSet.Update(entity);
        await _db.SaveChangesAsync();
        _disposer?.Push(entity);

        return new UpdateOutput<TEntity> {
            Original = original,
            Updated = entity,
        };
    }

    #endregion

    #region Delete

    /// <summary>
    ///     Deletes the <see cref="TEntity"/> record based on its <see cref="IEntity.Id"/> value.
    /// </summary>
    /// <param name="Id">
    ///     <see cref="IEntity.Id"/> to match.
    /// </param>
    /// <returns>
    ///     Deleted <see cref="TEntity"/> record.
    /// </returns>
    /// <exception cref="DepotError{TEntity}">
    ///     <see cref="IDepot{TEntity}"/> based exception, more info see inner Situation.
    /// </exception>
    public async Task<TEntity> Delete(long id) {
        TEntity entity = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id == id
            )
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{typeof(TEntity).Name}.Id = {id}");

        _dbSet.Remove(entity);
        _db.SaveChanges();
        return entity;
    }


    public async Task<BatchOperationOutput<TEntity>> Delete(long[] ids) {
        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];
        foreach (long id in ids) {

            try {
                TEntity success = await Delete(id);
                successes.Add(success);
            } catch (Exception ex) {
                failures.Add(
                        new EntityError<TEntity>(
                                EntityErrorEvents.DELETE_FAILED,
                                new TEntity {
                                    Id = id
                                },
                                ex
                            )
                    );
            }
        }

        return new BatchOperationOutput<TEntity>([.. successes], [.. failures]);
    }

    public async Task<BatchOperationOutput<TEntity>> Delete(QueryInput<TEntity, FilterQueryInput<TEntity>> input) {
        FilterQueryInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> query = _dbSet.Process(
                input,
                (query) => {
                    return query
                        .AsNoTracking()
                        .Where(parameters.Filter);
                }
            );

        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];

        TEntity[] entities = await query.ToArrayAsync();

        foreach (TEntity entity in entities) {
            try {
                TEntity deletedEntity = await Delete(entity.Id);
                successes.Add(deletedEntity);
            } catch (Exception exception) {
                failures.Add(
                        new EntityError<TEntity>(EntityErrorEvents.DELETE_FAILED, entity, exception)
                    );
            }
        }

        return new BatchOperationOutput<TEntity>([.. successes], [.. failures]);
    }

    public async Task<TEntity> Delete(TEntity Entity) {
        _dbSet.Remove(Entity);
        await _db.SaveChangesAsync();
        return Entity;
    }

    public async Task<BatchOperationOutput<TEntity>> Delete(TEntity[] entities) {
        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];
        foreach (TEntity entity in entities) {

            try {
                TEntity success = await Delete(entity);
                successes.Add(success);
            } catch (Exception ex) {
                failures.Add(
                        new EntityError<TEntity>(
                                EntityErrorEvents.DELETE_FAILED,
                                entity,
                                ex
                            )
                    );
            }
        }

        return new BatchOperationOutput<TEntity>([.. successes], [.. failures]);
    }

    #endregion
}