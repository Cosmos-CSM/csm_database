using CSharp_Database_Extension.Depots.Models;
using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents updating logic for a <see cref="IDepot{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> handled.
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
