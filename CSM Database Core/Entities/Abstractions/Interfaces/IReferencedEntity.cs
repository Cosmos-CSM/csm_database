using System.ComponentModel.DataAnnotations;

using CSM_Database_Core.Entities.Abstractions.Bases;

namespace CSM_Database_Core.Entities.Abstractions.Interfaces;

/// <summary>
///     Represents an <see cref="IEntity"/> with a <see cref="Reference"/> unique property wich allows its identification along
///     data storages migrations as this property is absolute static.
/// </summary>
public interface IReferencedEntity
    : IEntity {

    /// <summary>
    ///     Unique reference value.
    /// </summary>
    /// <remarks>
    ///     It's lenght must be strict 8.
    /// </remarks>
    [StringLength(8, MinimumLength = 8)]
    public string Reference { get; set; }
}