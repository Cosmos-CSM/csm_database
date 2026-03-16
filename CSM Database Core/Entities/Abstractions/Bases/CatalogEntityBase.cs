using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> with catalog identification properties.
/// </summary>
public abstract class CatalogEntityBase
    : NamedEntityBase, ICatalogEntity {

    /// <inheritdoc/>
    public string Reference { get; set; } = string.Empty;

    /// <inheritdoc/>
    public bool IsEnabled { get; set; }
}
