using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

using CSharp_Extension.Common.Abstractions.Interfaces;

namespace CSM_Database_Testing.Disposing.Abstractions.Interfaces;

/// <summary>
///     Represents a testing data disposer context handler.
/// </summary>
public interface ITestingDisposer
    : IDisposer<IEntity> {
}
