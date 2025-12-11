using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents reading logic for a <see cref="IDepot{TEntity, TEntityInterface}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> handled.
/// </typeparam>
/// <typeparam name="TEntityInterface">
///     Type of the <see cref="IEntity"/> interface handled.
/// </typeparam>
public interface IDepotRead<TEntity, TEntityInterface>
    where TEntity : class, TEntityInterface
    where TEntityInterface : IEntity {

    /// <summary>
    ///     Reads from data storages an <see cref="IEntity"/> with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">
    ///     Unique identifier to search for.
    /// </param>
    /// <returns>
    ///     Read output.
    /// </returns>
    Task<TEntityInterface> Read(long id);

    /// <summary>
    ///     Reads from data storages <see cref="IEntity"/> from the given <paramref name="ids"/>.
    /// </summary>
    /// <param name="ids">
    ///     Unique identifiers to search for.
    /// </param>
    /// <returns>
    ///     Batch read output.
    /// </returns>
    Task<BatchOperationOutput<TEntityInterface>> Read(long[] ids);

    /// <summary>
    ///     Reads from data storages <see cref="IEntity"/> matching the given <paramref name="input"/> filters.
    /// </summary>
    /// <param name="input">
    ///     Complex filtering query input.
    /// </param>
    /// <returns>
    ///     Bacth read output.
    /// </returns>
    Task<BatchOperationOutput<TEntityInterface>> Read(QueryInput<TEntityInterface, FilterQueryInput<TEntityInterface>> input);
}
