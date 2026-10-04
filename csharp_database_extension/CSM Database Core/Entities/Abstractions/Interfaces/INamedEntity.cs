using System.ComponentModel.DataAnnotations;

namespace CSM_Database_Core.Entities.Abstractions.Interfaces;

/// <summary>
///     Represents an <see cref="IEntity"/> with <see cref="Name"/> and <see cref="Description"/>
///     that can help to identify a <see cref="IEntity"/> based on <see cref="Name"/> property as this defines them as unique.
/// </summary>
public interface INamedEntity
    : IEntity {

    /// <summary>
    ///     Entity instance name.
    /// </summary>
    /// <remarks>
    ///     Length must be between 1 and 100.
    /// </remarks>
    [StringLength(100, MinimumLength = 1)]
    string Name { get; set; }

    /// <summary>
    ///     Entity instance description.
    /// </summary>
    string? Description { get; set; }
}
