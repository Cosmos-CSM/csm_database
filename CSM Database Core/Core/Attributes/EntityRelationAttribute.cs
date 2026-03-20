using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Core.Attributes;

/// <summary>
///     Attribute to mark a property as a relation from the main Entity.
/// </summary>
/// <remarks>
///     Mainly used on <see cref="IEntity"/> implementations for [CSM] automatic quality and featured 
///     functionalities for data handling.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
[Obsolete("This attribute is no longer supporter and will be removed at the next major (5.0.0), please use [EntityDependencyAttribute], [EntityDependantAttribute]", true)]
public class EntityRelationAttribute
    : Attribute {
}
