using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Interfaces;

namespace CSM_Database_Testing.Disposing.Abstractions.Interfaces;

/// <summary>
///     [Interface] for [Quality] purposes [Disposer] implementations.
/// </summary>
/// <remarks>
///     This Disposer only must be used on [Quality]/[Testing] strictly purposes.
/// </remarks>
public interface ITestingDisposer
    : IDisposer<IEntity> {
}
