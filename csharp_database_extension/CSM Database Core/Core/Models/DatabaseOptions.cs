using Microsoft.EntityFrameworkCore;

namespace CSM_Database_Core.Core.Models;

/// <summary>
///     Represents a <see cref="DatabaseBase{TDatabases}"/> building options.
/// </summary>
public partial record DatabaseOptions<TDatabase>
    where TDatabase : DbContext {

    /// <summary>
    ///     Whether the logging service is enabled.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    ///     Whether the database context building is for testing purposes.
    /// </summary>
    public bool ForTesting { get; init; } = false;

    /// <summary>
    ///     Database context signature.
    /// </summary>
    public string? Sign { get; set; }

    /// <summary>
    ///     Database connection options.
    /// </summary>
    public ConnectionOptions? ConnectionOptions { get; set; }

    /// <summary>
    ///     Native EntityFrameworkCore <see cref="DbContext"/> implementation options.
    /// </summary>
    public DbContextOptions<TDatabase>? DbContextOptions { get; set; }
}
