using System.Collections;
using System.Data;
using System.Reflection;

using CSM_Database_Core.Core.Attributes.Abstractions.Interfaces;
using CSM_Database_Core.Core.Errors;
using CSM_Database_Core.Core.Extensions;
using CSM_Database_Core.Core.Utils;
using CSM_Database_Core.Depots.Abstractions.Interfaces;
using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Depots.Models.Structs;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Interfaces;
using CSM_Foundation_Core.Core.Errors;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;

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
    ///     DBSet handler into <see cref="_db"/> to handle fastlike transactions related to the <typeparamref name="TEntity"/>.
    /// </summary>
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    ///     Generates a new instance of a <see cref="DepotBase{TDatabase, TEntity}"/> base.
    /// </summary>
    /// <param name="Database">
    ///     The <typeparamref name="TDatabase"/> that stores and handles the transactions for this <typeparamref name="TEntity"/> concept.
    /// </param>
    /// <param name="Disposer">
    ///     Data disposition manager (commonly and recommended use for testing data disposition).
    /// </param>
    public DepotBase(TDatabase Database, IDisposer<IEntity>? Disposer) {
        _db = Database;
        _disposer = Disposer;
        _dbSet = Database.Set<TEntity>();
    }

    /// <summary>
    ///     Validates that the given <paramref name="relation"/> is already created and valid in the system.
    /// </summary>
    /// <typeparam name="TRelationEntity">
    ///     Type of the relation entity to validate.
    /// </typeparam>
    /// <param name="relation">
    ///     Relation entity instance to validate.
    /// </param>
    /// <returns>
    ///     The instance when is valid, otherwise throws exception.
    /// </returns>
    /// <exception cref="Exception"></exception>
    protected TRelationEntity ValidateRelation<TRelationEntity>(TRelationEntity relation)
        where TRelationEntity : class, IEntity, new() {

        TRelationEntity? tmpRelation = relation;
        tmpRelation = tmpRelation.Id > 0
            ? _db.Set<TRelationEntity>().Where(dep => dep.Id == tmpRelation.Id).FirstOrDefault()
            : throw new Exception($"Dependencies aren't allowed to be auto-created on main Entity creation, you need to create the Dependency first in its corresponding [Depot]");

        return tmpRelation is null
            ? throw new Exception($"[{GetType().Name}] entity requires [{typeof(TRelationEntity)}] dependency")
            : tmpRelation;
    }

    #region View 

    /// <inheritdoc/>
    public async Task<ViewOutput<TEntity>> View(QueryInput<TEntity, ViewInput<TEntity>> input) {
        ViewInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> processedQuery = _dbSet.Process(
                input,
                (query) => {
                    processedQuery = query.OrderView(parameters.Orderings).Cast<TEntity>();
                    processedQuery = processedQuery.FilterView(parameters.Filters).Cast<TEntity>();

                    return processedQuery;
                }
            ).Cast<TEntity>();


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

    /// <inheritdoc/>
    public virtual async Task<TEntity> Create(TEntity entity) {
        TEntity instEntity = entity;

        instEntity.Timestamp = DateTime.UtcNow;
        instEntity.EvaluateWrite();

        instEntity = await DatabaseUtils.SanitizeEntity(_db, instEntity);
        await _dbSet.AddAsync(instEntity);

        _disposer?.Push(instEntity);
        await _db.SaveChangesAsync();

        return instEntity;
    }

    /// <inheritdoc/>
    public virtual async Task<BatchOperationOutput<TEntity>> Create(ICollection<TEntity> entities, bool sync = false) {
        IEnumerable<TEntity> instEntities = entities.Cast<TEntity>();

        TEntity[] createdEntities = [];
        EntityError<TEntity>[] errors = [];

        foreach (TEntity instEntity in instEntities) {
            try {
                TEntity attachedEntity = await Create(instEntity);
                createdEntities = [.. createdEntities, attachedEntity];
            } catch (Exception excep) {
                if (sync) {
                    throw;
                }

                EntityError<TEntity> error = new(EntityErrorEvents.CREATE_FAILED, instEntity, excep);
                errors = [.. errors, error];
            }
        }

        return new(createdEntities, errors);
    }

    #endregion

    #region Read

    /// <inheritdoc/>
    public async Task<TEntity> Read(long id) {
        TEntity? entity = await _dbSet.Where(
                e => e.Id == id
            )
            .FirstOrDefaultAsync()
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{nameof(IEntity.Id)} = {id}");

        entity.EvaluateRead();
        return entity;
    }

    /// <inheritdoc/>
    public async Task<BatchOperationOutput<TEntity>> Read(long[] ids) {

        List<TEntity> readings = [];
        List<EntityError<TEntity>> errors = [];

        foreach (long id in ids) {

            try {
                TEntity success = await Read(id);
                readings.Add(success);
            } catch (Exception ex) {
                errors.Add(
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

        return new BatchOperationOutput<TEntity>([.. readings], [.. errors]);
    }

    /// <inheritdoc/>
    public async Task<BatchOperationOutput<TEntity>> Read(QueryInput<TEntity, FilterQueryInput<TEntity>> input) {
        FilterQueryInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> processedQuery = _dbSet.Process(
                input,
                sourceQuery => {
                    sourceQuery = _dbSet.Where(parameters.Filter);
                    return sourceQuery;
                }
            )
            .Cast<TEntity>();

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
        List<EntityError<TEntity>> errors = [];
        foreach (TEntity item in resultItems) {
            try {
                item.EvaluateRead();
                successes.Add(item);
            } catch (Exception exception) {
                EntityError<TEntity> error = new(EntityErrorEvents.READ_VALIDATION_FAILED, item, exception);
                errors.Add(error);
            }
        }

        if (parameters.Behavior == FilteringBehaviors.First && errors.Count > 0) {
            throw errors[0];
        }

        return new BatchOperationOutput<TEntity>(
                [.. successes],
                [.. errors]
            );
    }

    #endregion

    #region Update 

    /// <inheritdoc/>
    public async Task<UpdateOutput<TEntity>> Update(QueryInput<TEntity, UpdateInput<TEntity>> input) {
        UpdateInput<TEntity> updateInput = input.Parameters;

        TEntity updateEntity = updateInput.Entity;
        // --> When the update entity comes with no ID, we inferr is to be created.
        if (updateEntity.Id <= 0) {
            if (!updateInput.Create)
                throw new DepotError<TEntity>(DepotErrorEvents.CREATE_DISABLED);

            updateEntity = await Create(updateEntity);
            _disposer?.Push(updateEntity);

            return new UpdateOutput<TEntity> {
                Original = default,
                Updated = updateEntity,
            };
        }

        TEntity? originalEntity = null;
        IQueryable<TEntity> query = _dbSet;

        IQueryable<TEntity> ogEntityQuery = query
            .AsNoTrackingWithIdentityResolution()
            .Where(obj => obj.Id == updateEntity.Id);

        if (input.PostProcessor != null) {
            ogEntityQuery = input.PostProcessor(ogEntityQuery);
        }

        originalEntity = await ogEntityQuery
            .FirstOrDefaultAsync();

        // --> When the update entity comes with ID, but there was no enitty stored with that ID, we check if creation is valid.
        if (originalEntity == null) {
            if (!updateInput.Create)
                throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{typeof(TEntity).Name}.Id = {updateEntity.Id}");

            updateEntity.Id = 0;
            updateEntity = await Create(updateEntity);
            _disposer?.Push(updateEntity);

            return new UpdateOutput<TEntity> {
                Original = default,
                Updated = updateEntity,
            };
        }

        // --> Here we know the operation is to update an existing entity.
        TEntity trackedEntity = await query
            .Where(obj => obj.Id == originalEntity.Id)
            .FirstOrDefaultAsync()
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND);

        // Updating scalar values.
        _db.Entry(trackedEntity).CurrentValues.SetValues(updateEntity);
        EntityBase trackedEntityBase = (trackedEntity as EntityBase)!;

        // Updating relations instructions.
        Dictionary<string, RelationUpdate[]> relationsUpdates = updateInput.Relations;
        if (!relationsUpdates.IsNullOrEmpty()) {

            foreach (KeyValuePair<string, RelationUpdate[]> relationUpdates in relationsUpdates) {
                string relationName = relationUpdates.Key;
                RelationUpdate[] updates = relationUpdates.Value;

                PropertyInfo relationPropertyInfo = trackedEntityBase.GetProperty(relationName);
                IEnumerable relationCollection = (IEnumerable)relationPropertyInfo.GetValue(trackedEntity)!;
                IEnumerable<IEntity> castedCollection = relationCollection.Cast<IEntity>();

                MethodInfo addMethod = relationCollection.GetType().GetMethod("Add")!;

                dynamic dynamicCollection = relationCollection;

                foreach (RelationUpdate update in updates) {
                    object? collectionItem = castedCollection.FirstOrDefault(
                            relationItem => relationItem.Id == update.Entity.Id
                        );

                    switch (update.Action) {
                        case RelationUpdateAction.ADD:
                            if (collectionItem is not null)
                                continue;

                            _db.Attach(update.Entity);
                            addMethod.Invoke(relationCollection, [update.Entity]);
                            break;
                        case RelationUpdateAction.REMOVE:
                            if (collectionItem is null)
                                continue;

                            dynamicCollection.Remove(collectionItem);
                            break;
                        default:
                            throw new NotImplementedException($"Relation Update action ({update.Action}) not implemented.");
                    }
                }
            }
        }

        // Updating relations that are not collections.
        Type entityType = typeof(TEntity);
        PropertyInfo[] entityTypeProperties = entityType.GetProperties();

        IEnumerable<PropertyInfo> flatRelations = entityTypeProperties
            .Where(
                    entityTypeProperty => {
                        IEnumerable<IRelationAttribute> attrs = entityTypeProperty
                            .GetCustomAttributes(inherit: true)
                            .OfType<IRelationAttribute>();


                        IRelationAttribute? attr = attrs.FirstOrDefault();
                        if (attr is null)
                            return false;

                        Type relType = entityTypeProperty.PropertyType;
                        bool isSingleRelation = relType.IsAssignableTo(typeof(IEntity));

                        return isSingleRelation;
                    }
                );

        // Updating no collection relations.
        foreach (PropertyInfo flatRelation in flatRelations) {

            object? updatedValue = flatRelation.GetValue(updateEntity);
            object? currentValue = flatRelation.GetValue(trackedEntity);

            if (updatedValue != currentValue) {
                flatRelation.SetValue(trackedEntity, updatedValue);
            }
        }

        await _db.SaveChangesAsync();


        IQueryable<TEntity> postQuery = query;

        // Applying hard auto-includes for post query.
        IEntityType dbProxyType = _db.Model.FindEntityType(typeof(TEntity))
            ?? throw new SystemError($"Unable to locate entity type ({typeof(TEntity).Name}) at database modeel ({_db.GetType().Name}).");

        IEnumerable<string>? autoLoadedNavs =
               dbProxyType
              .GetNavigations()
              .Where(n => n.IsEagerLoaded)
              .Select(
                        nav => nav.Name
                   );

        foreach (string autoLoadedNav in autoLoadedNavs) {
            postQuery.Include(autoLoadedNav);
        }

        if (input.PostProcessor is not null) {
            postQuery = input.PostProcessor(postQuery);
        }

        updateEntity = await postQuery
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(
                    obj => obj.Id == trackedEntity.Id
                )
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{typeof(TEntity).Name}.Id = {trackedEntity.Id}");

        return new UpdateOutput<TEntity> {
            Original = originalEntity,
            Updated = updateEntity,
        };
    }

    #endregion

    #region Delete

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<BatchOperationOutput<TEntity>> Delete(QueryInput<TEntity, FilterQueryInput<TEntity>> input) {
        FilterQueryInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> query = _dbSet.Process(
                input,
                (query) => {
                    return query
                        .Cast<TEntity>()
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

    /// <inheritdoc/>
    public async Task<TEntity> Delete(TEntity entity) {
        _dbSet.Remove(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    /// <inheritdoc/>
    public async Task<BatchOperationOutput<TEntity>> Delete(TEntity[] entities) {
        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];
        foreach (TEntity entity in entities.Cast<TEntity>()) {

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