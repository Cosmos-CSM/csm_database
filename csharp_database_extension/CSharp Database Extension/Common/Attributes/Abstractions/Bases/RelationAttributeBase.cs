using CSharp_Database_Extension.Core.Attributes.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Core.Attributes.Abstractions.Bases;

/// <inheritdoc cref="IRelationAttribute"/>
public abstract class RelationAttributeBase
     : Attribute, IRelationAttribute {

    /// <inheritdoc/>
    public string? Name { get; set; }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="name">
    ///     Relation's name.
    /// </param>
    public RelationAttributeBase(string? name = null) {
        Name = name;
    }
}
