namespace CSharp_Database_Extension.Abstractions.Interfaces;

/// <summary>
///     Represents a Database model.
/// </summary>
public interface IDatabase {

    /// <summary>
    ///     Database signature, used to be identified along ecosystems.
    /// </summary>
    string Signature { get; }

    /// <summary>
    ///     Validates database connection and configuration health.
    /// </summary>
    /// <param name="strict">
    ///     Whether the validation process is strict, breaking execution on error.
    /// </param>
    /// <returns>
    ///     Whether the validation process was sucessful or not.
    /// </returns>
    bool Validate(bool strict = true);
}