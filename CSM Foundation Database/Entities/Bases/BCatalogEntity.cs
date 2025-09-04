using CSM_Foundation.Database;

namespace CSM_Foundation_Database.Entities.Bases;


/// <summary>
///     Represents an <see cref="IEntity"/> with catalog identification properties.
/// </summary>
public interface ICatalogEntity
    : IEntity, INamedEntity, IReferencedEntity, IActivableEntity {
}

/// <summary>
///     Represents an <see cref="IEntity"/> with catalog identification properties.
/// </summary>
public abstract class BCatalogEntity
    : BEntity, ICatalogEntity {

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Reference { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
