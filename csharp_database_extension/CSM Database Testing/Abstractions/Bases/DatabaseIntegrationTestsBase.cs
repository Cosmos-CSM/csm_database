using CSM_Database_Core;
using CSM_Database_Core.Core.Models;

using CSM_Database_Testing.Abstractions.Interfaces;

using CSM_Foundation_Core.Core.Extensions;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace CSM_Database_Testing.Abstractions.Bases;

/// <summary>
///     Represents a testing class for a Database context.
/// </summary>
/// <typeparam name="TDatabase">
///     Database context type to be tested.
/// </typeparam>
public abstract class DatabaseIntegrationTestsBase<TDatabase>
    : ITestingDatabase
    where TDatabase : DatabaseBase<TDatabase>, new() {

    /// <summary>
    ///     Database context instance.  
    /// </summary>
    protected readonly TDatabase _database;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public DatabaseIntegrationTestsBase() {

        _database = (TDatabase)Activator.CreateInstance(
                typeof(TDatabase),
                new DatabaseOptions<TDatabase> {
                    ForTesting = true,
                }
            )!;
    }

    /// <summary>
    ///     Tests that <typeparamref name="TDatabase"/> is correctly migrated at the target server.
    /// </summary>
    [Fact]
    public void Migration() {
        IEnumerable<string> pendingMigrations = _database.Database.GetPendingMigrations();

        Assert.True(pendingMigrations.Empty(), $"Database instance isn't up-to-date with current database migrations. ({pendingMigrations.Count()} pendent)");
    }

    /// <summary>
    ///     Tests that <typeparamref name="TDatabase"/> has communication with the server.
    /// </summary>
    [Fact]
    public void Communication() {
        Assert.True(_database.Database.CanConnect(), $"{GetType()} cannot connect, check your connection credentials.");
    }

    /// <summary>
    ///     Testst that <typeparamref name="TDatabase"/> is correctly configured
    /// </summary>
    [Fact]
    public void Evaluate() {
        _database.Validate();
    }
}
