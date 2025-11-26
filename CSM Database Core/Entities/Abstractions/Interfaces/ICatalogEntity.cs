namespace CSM_Database_Core.Entities.Abstractions.Interfaces;


/// <summary>
///     Represents an <see cref="IEntity"/> with catalog identification properties.
/// </summary>
public interface ICatalogEntity
    : IEntity, INamedEntity, IReferencedEntity, IActivableEntity {
}
