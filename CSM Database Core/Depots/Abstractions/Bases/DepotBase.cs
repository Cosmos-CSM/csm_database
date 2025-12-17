using System.Data;

using CSM_Database_Core.Core.Errors;
using CSM_Database_Core.Core.Extensions;
using CSM_Database_Core.Core.Utils;
using CSM_Database_Core.Depots.Abstractions.Interfaces;
using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore;

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

    public virtual async Task<TEntity> Create(TEntity entity) {
        TEntity instEntity = (TEntity)entity;

        instEntity.Timestamp = DateTime.UtcNow;
        instEntity.EvaluateWrite();

        instEntity = DatabaseUtils.SanitizeEntity(_db, instEntity);
        await _dbSet.AddAsync(instEntity);

        _disposer?.Push(instEntity);
        await _db.SaveChangesAsync();

        return instEntity;
    }

    public virtual async Task<BatchOperationOutput<TEntity>> Create(ICollection<TEntity> entities, bool sync = false) {
        IEnumerable<TEntity> instEntities = entities.Cast<TEntity>();

        TEntity[] createdEntities = [];
        EntityError<TEntity>[] errors = [];

        foreach (TEntity instEntity in instEntities) {
            try {
                TEntity attachedEntity = await Create(instEntity);
                createdEntities = [.. createdEntities, (TEntity)attachedEntity];
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

        List<TEntity> readings = [];
        List<EntityError<TEntity>> errors = [];

        foreach (long id in ids) {

            try {
                TEntity success = (TEntity)await Read(id);
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

    public async Task<UpdateOutput<TEntity>> Update(QueryInput<TEntity, UpdateInput<TEntity>> input) {
        UpdateInput<TEntity> parameters = input.Parameters;

        IQueryable<TEntity> processedQuery = _dbSet.Process(
                input,
                (sourceQuery) => sourceQuery
            )
            .Cast<TEntity>();

        TEntity entity = (TEntity)parameters.Entity;

        /// --> When the entity is not saved yet.
        if (entity.Id == 0) {
            if (!parameters.Create) {
                throw new DepotError<TEntity>(DepotErrorEvents.CREATE_DISABLED);
            }

            entity = (TEntity)await Create(entity);
            _disposer?.Push(entity);

            return new UpdateOutput<TEntity> {
                Original = default,
                Updated = entity,
            };
        }


        TEntity? original = await processedQuery
            .Where(obj => obj.Id == entity.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync()
            ?? throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND);

        if (original == null) {
            if (!parameters.Create)
                throw new DepotError<TEntity>(DepotErrorEvents.UNFOUND, $"{typeof(TEntity).Name}.Id = {entity.Id}");

            entity.Id = 0;
            entity = (TEntity)await Create(entity);
            _disposer?.Push(entity);

            return new UpdateOutput<TEntity> {
                Original = default,
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
                TEntity success = (TEntity)await Delete(id);
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
                TEntity deletedEntity = (TEntity)await Delete(entity.Id);
                successes.Add(deletedEntity);
            } catch (Exception exception) {
                failures.Add(
                        new EntityError<TEntity>(EntityErrorEvents.DELETE_FAILED, entity, exception)
                    );
            }
        }

        return new BatchOperationOutput<TEntity>([.. successes], [.. failures]);
    }

    public async Task<TEntity> Delete(TEntity entity) {
        _dbSet.Remove((TEntity)entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<BatchOperationOutput<TEntity>> Delete(TEntity[] entities) {
        List<TEntity> successes = [];
        List<EntityError<TEntity>> failures = [];
        foreach (TEntity entity in entities.Cast<TEntity>()) {

            try {
                TEntity success = (TEntity)await Delete(entity);
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