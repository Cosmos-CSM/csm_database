using CSharp_Database_Extension.Depots.Models;
using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents a [View] operations logic for a <see cref="IDepot{TEntity}"/>
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> type hanlded for the <see cref="IDepot{TEntity}"/>.
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
