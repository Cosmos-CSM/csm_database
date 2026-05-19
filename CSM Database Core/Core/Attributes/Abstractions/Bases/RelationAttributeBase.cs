using CSM_Database_Core.Core.Attributes.Abstractions.Interfaces;

using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Core.Attributes.Abstractions.Bases;

/// <inheritdoc cref="IRelationAttribute"/>
public abstract class RelationAttributeBase
     : Attribute, IRelationAttribute {

    /// <inheritdoc/>
    public Type Type { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public bool IsCollection { get; set; } = false;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="name">
    ///     Relation's name.
    /// </param>
    /// <param name="type">
    ///     Relation's <see cref="IEntity"/> type;
    /// </param>
    /// <param name="isCollection">
    ///     Whether the relation is a collection.
    /// </param>
    public RelationAttributeBase(string name, Type type, bool isCollection = false) {
        Type = type;
        Name = name;
        IsCollection = isCollection;
    }
}
