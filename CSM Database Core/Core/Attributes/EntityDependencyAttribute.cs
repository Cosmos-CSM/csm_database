namespace CSM_Database_Core.Core.Attributes;

/// <summary>
///     Attribute to mark a relation dependency.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EntityDependencyAttribute
    : Attribute {
}
