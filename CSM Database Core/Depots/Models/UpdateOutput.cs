using CSM_Database_Core.Entities.Abstractions.Bases;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Models;

/// <summary>
///     [Record] for the output of an entity [Update] operation.
/// </summary>
/// <typeparam name="T"></typeparam>
public record UpdateOutput<T>
    where T : IEntity {
    /// <summary>
    ///     The new updated <typeparamref name="T"/> instance.
    /// </summary>
    public required T Updated { get; set; }

    /// <summary>
    ///     The original <typeparamref name="T"/> instance before the update operation.
    /// </summary>
    /// <remarks>
    ///     This property depends on the operation parameters and if there was an original entity before the update operation.
    /// </remarks>
    public T? Original { get; set; }
}
