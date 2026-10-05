using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Interfaces;

namespace CSM_Database_Testing.Disposing.Abstractions.Interfaces;

/// <summary>
///     Represents a testing data disposer context handler.
/// </summary>
public interface ITestingDisposer
    : IDisposer<IEntity> {
}
