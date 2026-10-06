using CSharp_Database_Extension.Common.Enums;

using Microsoft.EntityFrameworkCore;

namespace CSharp_Database_Extension.Common.Models;

/// <summary>
///     Represents a <see cref="DatabaseBase{TDatabases}"/> building options.
/// </summary>
public partial record DatabaseOptions<TDatabase>
    where TDatabase : DbContext {

    /// <summary>
    ///     Database signature.
    /// </summary>
    public string? Signature { get; set; }

    /// <summary>
    ///     Database provider.
    /// </summary>
    public DatabaseProviders Provider { get; set; } = DatabaseProviders.PostgreSQL;

    /// <summary>
    ///     Whether the logging service is enabled.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    ///     Whether the database context building is for testing purposes.
    /// </summary>
    public bool ForTesting { get; init; } = false;

    /// <summary>
    ///     Database connection options.
    /// </summary>
    public ConnectionOptions? ConnectionOptions { get; set; }

    /// <summary>
    ///     Native EntityFrameworkCore <see cref="DbContext"/> implementation options.
    /// </summary>
    public DbContextOptions<TDatabase>? DbContextOptions { get; set; }
}
