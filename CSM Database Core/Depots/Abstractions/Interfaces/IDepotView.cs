using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents a [View] operations logic for a <see cref="IDepot{TEntity, TEntity}"/>
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> type hanlded for the <see cref="IDepot{TEntity, TEntity}"/>.
/// </typeparam>
/// <typeparam name="TEntity">
///     Type of the interface that represents the <see cref="IEntity"/> type implementation handled from the <see cref="IDepot{TEntity, TEntity}"/>
/// </typeparam>
public interface IDepotView<TEntity>
    where TEntity : class, IEntity {

    /// <summary>
    ///     Creates a complex view over the given <typeparamref name="TEntity"/> data.
    /// </summary>
    /// <param name="input">
    ///     View input.
    /// </param>
    /// <returns>
    ///     View output.
    /// </returns>
    Task<ViewOutput<TEntity>> View(QueryInput<TEntity, ViewInput<TEntity>> input);
}
