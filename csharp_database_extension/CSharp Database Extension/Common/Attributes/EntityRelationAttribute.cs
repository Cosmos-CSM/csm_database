using CSharp_Database_Extension.Common.Attributes.Abstractions.Bases;

namespace CSharp_Database_Extension.Common.Attributes;

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
