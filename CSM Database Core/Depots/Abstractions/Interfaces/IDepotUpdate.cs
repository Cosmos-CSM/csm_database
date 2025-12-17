using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents updating logic for a <see cref="IDepot{TEntity, TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> handled.
/// </typeparam>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> interface handled.
/// </typeparam>
public interface IDepotUpdate<TEntity>
    where TEntity : class, IEntity {

    /// <summary>
    ///     Updates from data storages based on the given <paramref name="input"/>.
    /// </summary>
    /// <param name="input">
    ///     Update input.
    /// </param>
    /// <returns>
    ///     Update output.
    /// </returns>
    Task<UpdateOutput<TEntity>> Update(QueryInput<TEntity, UpdateInput<TEntity>> input);
}
