using CSM_Database_Core.Entities.Abstractions.Interfaces;
using CSM_Database_Core.Validation.Abstractions.Bases;

namespace CSM_Database_Testing.Abstractions.Bases;

/// <summary>
///     Represents an integration tests class for <see cref="IEntity"/> implementations.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> tested.
/// </typeparam>
public record EntityIntegrationTestsBase<TEntity>
    where TEntity : IEntity {

    /// <summary>
    ///     Expected results based on the given <see cref="ValidatorBase"/> checks.
    /// </summary>
    public (string, (ValidatorBase, int)[])[] Expectations { get; init; } = [];

    /// <summary>
    ///     Entity mock.
    /// </summary>
    public TEntity Mock { get; init; } = default!;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="Mock">
    ///     Entity mock.
    /// </param>
    /// <param name="Expectations">
    ///     Expected results based on the given <see cref="ValidatorBase"/> checks.
    /// </param>
    public EntityIntegrationTestsBase(TEntity Mock, (string, (ValidatorBase, int)[])[] Expectations) {
        this.Mock = Mock;
        this.Expectations = Expectations;
    }
}
