using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents deleting logic for a <see cref="IDepot{TEntity, TEntityInterface}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> handled.
/// </typeparam>
/// <typeparam name="TEntityInterface">
///     Type of the <see cref="IEntity"/> interface handled.
/// </typeparam>
public interface IDepotDelete<TEntity, TEntityInterface>
    where TEntity : class, TEntityInterface
    where TEntityInterface : IEntity {

    /// <summary>
    ///     Deletes from data storages a(n) <typeparamref name="TEntity"/> based on the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    ///     Unique identifier to search for the <see cref="TEntity"/>.
    /// </param>
    /// <returns>
    ///     Deleted <see cref="IEntity"/> data.
    /// </returns>
    public Task<TEntityInterface> Delete(long id);

    /// <summary>
    ///     Deletes from data storages a collection of <typeparamref name="TEntity"/> based on the given <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">
    ///     Unique identifiers to search for the <see cref="TEntity"/>(s).
    /// </param>
    /// <returns>
    ///     Batch delete output.
    /// </returns>
    public Task<BatchOperationOutput<TEntityInterface>> Delete(long[] ids);

    /// <summary>
    ///     Deletes from data storages the given <paramref name="entity"/>.
    /// </summary>
    /// <param name="entity">
    ///     <typeparamref name="TEntity"/> to delete.
    /// </param>
    /// <returns>
    ///     Deleted <typeparamref name="TEntity"/> data.
    /// </returns>
    public Task<TEntityInterface> Delete(TEntityInterface entity);

    /// <summary>
    ///     Deletes from data storages the given collection of <paramref name="entities"/>.
    /// </summary>
    /// <param name="entities">
    ///     Collection of <typeparamref name="TEntity"/> to delete.
    /// </param>
    /// <returns>
    ///     Batch delete output.
    /// </returns>
    public Task<BatchOperationOutput<TEntityInterface>> Delete(TEntityInterface[] entities);

    /// <summary>
    ///     Deletes from data storages <typeparamref name="TEntity"/> based on the given <paramref name="input"/> filters.
    /// </summary>
    /// <param name="input">
    ///     Complex filtering input.
    /// </param>
    /// <returns>
    ///     Batch delete output.
    /// </returns>
    public Task<BatchOperationOutput<TEntityInterface>> Delete(QueryInput<TEntityInterface, FilterQueryInput<TEntityInterface>> input);
}
