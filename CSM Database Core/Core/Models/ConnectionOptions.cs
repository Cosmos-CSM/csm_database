namespace CSM_Database_Core.Core.Models;

/// <summary>
///     Represents a database server connection options.
/// </summary>
public record ConnectionOptions {

    /// <summary>
    ///     Server host address.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    ///     Database name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Database server user.
    /// </summary>
    public required string User { get; init; }

    /// <summary>
    ///     Database server password.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    ///     Whether the connection is encrypted.
    /// </summary>
    public bool Encrypt { get; init; } = false;

    /// <summary>
    ///     Whether connection use integrated security (windows).
    /// </summary>
    public bool IntegratedSecurity { get; init; } = false;

    /// <summary>
    ///     Whether connection certificate is trusted.
    /// </summary>
    public bool Trust { get; init; } = false;

    /// <summary>
    ///     Wheter MARS is enabled.
    /// </summary>
    public bool MARS { get; init; } = false;

    /// <summary>
    ///    Database connection formatted string.
    /// </summary>
    public string ConnectionString {
        get {
            string connectionString = $"Server={Host};Database={Name};";

            if (IntegratedSecurity) {
                connectionString += $"Integrated Security={IntegratedSecurity};";
            } else {
                connectionString += $"User={User};Password={Password};";
            }

            return connectionString + $"Encrypt={Encrypt};TrustServerCertificate={Trust};MultipleActiveResultSets={MARS};";
        }
    }
}
