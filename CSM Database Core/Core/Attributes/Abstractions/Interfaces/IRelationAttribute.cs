namespace CSM_Database_Core.Core.Attributes.Abstractions.Interfaces;

using CSM_Database_Core.Entities.Abstractions.Interfaces;

/// <summary>
///     Represents an <see cref="IEntity"/> relation attribute.
/// </summary>
public interface IRelationAttribute {

    /// <summary>
    ///     Relation <see cref="IEntity"/> type.
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    ///     Relation name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Whether the relation is a collection.
    /// </summary>
    public bool IsCollection { get; set; }
}
