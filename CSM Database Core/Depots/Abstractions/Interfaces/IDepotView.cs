using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents a [View] operations logic for a <see cref="IDepot{TEntity, TEntityInterface}"/>
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> type hanlded for the <see cref="IDepot{TEntity, TEntityInterface}"/>.
/// </typeparam>
/// <typeparam name="TEntityInterface">
///     Type of the interface that represents the <see cref="IEntity"/> type implementation handled from the <see cref="IDepot{TEntity, TEntityInterface}"/>
/// </typeparam>
public interface IDepotView<TEntity, TEntityInterface>
    where TEntity : class, TEntityInterface
    where TEntityInterface : IEntity {

    /// <summary>
    ///     Creates a complex view over the given <typeparamref name="TEntity"/> data.
    /// </summary>
    /// <param name="input">
    ///     View input.
    /// </param>
    /// <returns>
    ///     View output.
    /// </returns>
    Task<ViewOutput<TEntityInterface>> View(QueryInput<TEntityInterface, ViewInput<TEntityInterface>> input);
}
