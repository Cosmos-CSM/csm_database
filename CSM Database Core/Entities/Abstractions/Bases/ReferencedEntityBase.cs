using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> with a <see cref="IReferencedEntity.Reference"/> unique property wich allows its identification along
///     data storages migrations as this property is absolute static.
/// </summary>
public abstract class ReferencedEntityBase
    : EntityBase, IReferencedEntity {

    public string Reference { get; set; } = string.Empty;
}