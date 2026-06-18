
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Core.Attributes.Abstractions.Interfaces;


/// <summary>
///     Represents an <see cref="IEntity"/> relation attribute.
/// </summary>
public interface IRelationAttribute {

    /// <summary>
    ///     Relation name.
    /// </summary>
    public string? Name { get; set; }
}
