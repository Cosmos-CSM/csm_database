using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Depots.Models.Structs;

/// <summary>
///     Represents available actions to perform for a <see cref="RelationUpdate"/>.
/// </summary>
public enum RelationUpdateAction {
    /// <summary>
    ///     Used to determine the relation is to be added.
    /// </summary>
    ADD,

    /// <summary>
    ///     Used to determine the relation is to be removed.
    /// </summary>
    REMOVE,
}


/// <summary>
///     Represents an <see cref="UpdateInput{TEntity}"/> relation update instruction.
/// </summary>
public struct RelationUpdate {
    /// <summary>
    ///     Relation entity.
    /// </summary>
    public IEntity Entity { get; set; }

    /// <summary>
    ///     Action to perform for the relation update.
    /// </summary>
    public RelationUpdateAction Action { get; set; }
}
