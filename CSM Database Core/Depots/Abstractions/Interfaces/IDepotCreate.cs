using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Abstractions.Interfaces;


/// <summary>
///     Represents create logic for a <see cref="IDepot{TEntity, TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity""/> type being handled.
/// </typeparam>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> interface type being handled.
/// </typeparam>
public interface IDepotCreate<TEntity>
    where TEntity : class, IEntity {


    /// <summary>
    ///     Creates a new <paramref name="entity"/> into data storages.
    /// </summary>
    /// <param name="entity">
    ///     Entity instance to create.
    /// </param>
    /// <returns> 
    ///     Created Entity instance.
    /// </returns>
    Task<TEntity> Create(TEntity entity);

    /// <summary>
    ///     Creates new <paramref name="entities"/> into data storages.
    /// </summary>
    /// <param name="entities">
    ///     Entity instances to create.
    /// </param>
    /// <param name="sync">
    ///     Whether the operation must throw an exception at the first failure.
    /// </param>
    /// <returns>
    ///     Batch create output.
    /// </returns>
    Task<BatchOperationOutput<TEntity>> Create(ICollection<TEntity> entities, bool sync = false);
}
