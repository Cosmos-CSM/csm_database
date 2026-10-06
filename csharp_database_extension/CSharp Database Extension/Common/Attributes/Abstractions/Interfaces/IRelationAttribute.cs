using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Core.Attributes.Abstractions.Interfaces;


/// <summary>
///     Represents an <see cref="IEntity"/> relation attribute.
/// </summary>
public interface IRelationAttribute {

    /// <summary>
    ///     Relation name.
    /// </summary>
    public string? Name { get; set; }
}
