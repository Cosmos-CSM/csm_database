using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> with catalog identification properties.
/// </summary>
public abstract class CatalogEntityBase
    : EntityBase, ICatalogEntity {

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Reference { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
