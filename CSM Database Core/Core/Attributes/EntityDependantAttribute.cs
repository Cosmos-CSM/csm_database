using CSM_Database_Core.Core.Attributes.Abstractions.Bases;

namespace CSM_Database_Core.Core.Attributes;

/// <summary>
///     Attribute to mark a relation dependant.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EntityDependantAttribute
    : RelationAttributeBase {

    /// <inheritdoc/>
    public EntityDependantAttribute(string name, Type type, bool isCollection = false)
        : base(name, type, isCollection) {
    }
}
