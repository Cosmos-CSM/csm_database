using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> with enablement control.
/// </summary>
public abstract class ActivableEntityBase
    : EntityBase, IEntity, IActivableEntity {

    public bool IsEnabled { get; set; } = false;
}
