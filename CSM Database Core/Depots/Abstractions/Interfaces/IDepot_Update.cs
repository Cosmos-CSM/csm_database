using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Bases;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;

/// <summary>
///     [Interface] describing [Update] actions for [Depot] implementations.
/// </summary>
/// <typeparam name="TEntity">
///     [Entity] type for the [Depot] implementation.
/// </typeparam>
public interface IDepot_Update<TEntity>
    where TEntity : class, IEntity {

    /// <summary>
    ///     Updates the given record calculating the current stored values with the given <paramref name="entity"/> to update and store the new values.
    /// </summary>
    /// <param name="Input">
    ///     Operation input parameters.
    /// </param>
    /// <returns></returns>
    /// <remarks>
    ///     Always the record to be overriden will be defined by the <see cref="IEntity.Id"/> property, if isn't given, will try with <see cref="NamedEntityBase.Name"/> property in case the
    ///     [Entity] implementation does have it, otherwise will finally create a new record with the given values.
    /// </remarks>
    Task<UpdateOutput<TEntity>> Update(QueryInput<TEntity, UpdateInput<TEntity>> Input);
}
