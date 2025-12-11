using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents updating logic for a <see cref="IDepot{TEntity, TEntityInterface}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> handled.
/// </typeparam>
/// <typeparam name="TEntityInterface">
///     Type of the <see cref="IEntity"/> interface handled.
/// </typeparam>
public interface IDepotUpdate<TEntity, TEntityInterface>
    where TEntity : class, TEntityInterface
    where TEntityInterface : IEntity {

    /// <summary>
    ///     Updates from data storages based on the given <paramref name="input"/>.
    /// </summary>
    /// <param name="input">
    ///     Update input.
    /// </param>
    /// <returns>
    ///     Update output.
    /// </returns>
    Task<UpdateOutput<TEntityInterface>> Update(QueryInput<TEntityInterface, UpdateInput<TEntityInterface>> input);
}
