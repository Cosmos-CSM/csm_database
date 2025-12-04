using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     [Delegate] declaration to expose an easier API to generate the accumulative instructions for the query after filtering.
/// </summary>
/// <typeparam name="TEntity">
///     [<see cref="IEntity"/>] implementation class type.
/// </typeparam>
/// <param name="query">
///     The proxied query to apply accumulative instructions.
/// </param>
/// <returns>
///     Must return the accumulated query as <see cref="IQueryable{T}"/> object is immutable.
/// </returns>
public delegate IQueryable<TEntity> QueryProcessor<TEntity>(IQueryable<TEntity> query) where TEntity : IEntity;

/// <summary>
///     Represents a database entity depot, handling entity operations.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the depot entity handled.
/// </typeparam>
public interface IDepot<TEntity, TEntityInterface>
    : IDepot_View<TEntity, TEntityInterface>
    , IDepot_Read<TEntity>
    , IDepot_Create<TEntity>
    , IDepot_Update<TEntity>
    , IDepot_Delete<TEntity>
    where TEntity : class, TEntityInterface
    where TEntityInterface : IEntity { }
