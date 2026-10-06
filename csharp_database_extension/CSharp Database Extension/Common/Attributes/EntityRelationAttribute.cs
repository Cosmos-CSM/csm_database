using CSharp_Database_Extension.Core.Attributes.Abstractions.Bases;

namespace CSharp_Database_Extension.Core.Attributes;

/// <summary>
///     Attribute to mark a relation dependency.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EntityRelationAttribute
    : RelationAttributeBase {

    /// <inheritdoc/>
    public EntityRelationAttribute(string? name = null)
        : base(name) {
    }
}
