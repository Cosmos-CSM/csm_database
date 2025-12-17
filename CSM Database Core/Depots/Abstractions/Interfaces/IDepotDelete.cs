using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents deleting logic for a <see cref="IDepot{TEntity, TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> handled.
/// </typeparam>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> interface handled.
/// </typeparam>
public interface IDepotDelete<TEntity>
    where TEntity : class, IEntity {

    /// <summary>
    ///     Deletes from data storages a(n) <typeparamref name="TEntity"/> based on the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    ///     Unique identifier to search for the <see cref="TEntity"/>.
    /// </param>
    /// <returns>
    ///     Deleted <see cref="IEntity"/> data.
    /// </returns>
    public Task<TEntity> Delete(long id);

    /// <summary>
    ///     Deletes from data storages a collection of <typeparamref name="TEntity"/> based on the given <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">
    ///     Unique identifiers to search for the <see cref="TEntity"/>(s).
    /// </param>
    /// <returns>
    ///     Batch delete output.
    /// </returns>
    public Task<BatchOperationOutput<TEntity>> Delete(long[] ids);

    /// <summary>
    ///     Deletes from data storages the given <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">
    ///     <typeparamref name="TEntity"/> to delete.
    /// </param>
    /// <returns>
    ///     Deleted <typeparamref name="TEntity"/> data.
    /// </returns>
    public Task<TEntity> Delete(TEntity entity);

    /// <summary>
    ///     Deletes from data storages the given collection of <paramref name="entities"/>.
    /// </summary>
    /// <param name="entities">
    ///     Collection of <typeparamref name="TEntity"/> to delete.
    /// </param>
    /// <returns>
    ///     Batch delete output.
    /// </returns>
    public Task<BatchOperationOutput<TEntity>> Delete(TEntity[] entities);

    /// <summary>
    ///     Deletes from data storages <typeparamref name="TEntity"/> based on the given <paramref name="input"/> filters.
    /// </summary>
    /// <param name="input">
    ///     Complex filtering input.
    /// </param>
    /// <returns>
    ///     Batch delete output.
    /// </returns>
    public Task<BatchOperationOutput<TEntity>> Delete(QueryInput<TEntity, FilterQueryInput<TEntity>> input);
}
