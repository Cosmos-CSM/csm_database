using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> with <see cref="Name"/> and <see cref="Description"/> properties
///     that can help to identify a <see cref="IEntity"/> based on <see cref="Name"/> property as this defines them as unique.
/// </summary>
public abstract class NamedEntityBase
    : EntityBase, INamedEntity {

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}