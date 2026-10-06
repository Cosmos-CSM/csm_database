using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Depots.Abstractions.Interfaces;

/// <summary>
///     Represents an Entity shpaed update instructions input.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IUpdateEntityInput<TEntity>
    where TEntity : IEntity {

    /// <summary>
    ///     Wheter the record should be created if it doesn't exist in the database.
    /// </summary>
    public bool Create { get; init; }

    /// <summary>
    ///     Entity being updated, scalar types and id is got from here.
    /// </summary>
    public TEntity Entity { get; init; }

    /// <summary>
    ///     Instruction to apply changes to the tracked entity's relations.
    /// </summary>
    /// <param name="trackedEntity">
    ///     DB Tracked entity.
    /// </param>
    public void UpdateRelations(TEntity trackedEntity);
}
