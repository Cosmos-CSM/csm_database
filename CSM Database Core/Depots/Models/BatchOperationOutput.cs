using CSM_Database_Core.Core.Errors;
using CSM_Database_Core.Entities.Abstractions.Bases;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

namespace CSM_Database_Core.Depots.Models;

/// <summary>
///     [Record] that stores the information about a batch operation output result.
/// </summary>
/// <typeparam name="T">
///     Type of the <see cref="IEntity"/> affected by the operations.
/// </typeparam>
public record BatchOperationOutput<T>
    where T : class, IEntity {

    /// <summary>
    ///     Collection of batch operation successes.
    /// </summary>
    public T[] Successes { get; init; }

    /// <summary>
    ///     Collection of batch operation failures. 
    /// </summary>
    public EntityError<T>[] Failures { get; init; }

    /// <summary>
    ///     Whether at least one operation iteration has failed.
    /// </summary>
    public bool Failed { get; private set; }

    /// <summary>
    ///     Whether all operations have failed.   
    /// </summary>
    public bool FullFailed { get; private set; }

    /// <summary>
    ///     The total amount of operations executed.
    /// </summary>
    public int OperationsCount { get; private set; }

    /// <summary>
    ///    The total amount of failed operations.
    /// </summary>
    public int FailuresCount { get; private set; }

    /// <summary>
    ///    The total amount of successful operations.
    /// </summary>
    public int SuccessesCount { get; private set; }

    public BatchOperationOutput(T[] Successes, EntityError<T>[] Failures) {
        this.Successes = Successes;
        this.Failures = Failures;

        SuccessesCount = this.Successes.Length;
        FailuresCount = this.Failures.Length;
        OperationsCount = SuccessesCount + FailuresCount;

        Failed = FailuresCount > 0;
        FullFailed = OperationsCount == FailuresCount;
    }
}