namespace CSM_Database_Core.Abstractions.Interfaces;

/// <summary>
///     Represents a Database context.
/// </summary>
public interface IDatabase {

    /// <summary>
    ///     Database unique identification sign.
    /// </summary>
    string Sign { get; }

    /// <summary>
    ///     Validates database connection and configuration health.
    /// </summary>
    /// <param name="strict">
    ///     Whether the validation process is strict, throwing exceptions on failures.
    /// </param>
    /// <returns>
    ///     Whether the validation process was sucessful or not.
    /// </returns>
    bool Validate(bool strict = true);
}