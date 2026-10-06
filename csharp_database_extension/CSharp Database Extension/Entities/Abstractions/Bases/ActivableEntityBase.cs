using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> with enablement control.
/// </summary>
public abstract class ActivableEntityBase
    : EntityBase, IEntity, IActivableEntity {

    /// <inheritdoc/>
    public bool IsEnabled { get; set; } = false;
}
