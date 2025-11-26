using CSM_Database_Core;
using CSM_Database_Core.Core.Utilitites;

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
public abstract class TestingDatabaseBase<TDatabase>
    : ITestingDatabase
    where TDatabase : DatabaseBase<TDatabase> {

    /// <summary>
    ///     Database context instance.  
    /// </summary>
    protected readonly TDatabase _database;

    /// <summary>
    ///     Creates a new <see cref="TestingDatabaseBase{TDatabase}"/> instance.
    /// </summary>
    /// <param name="Sign">
    ///     Custom identifier for multiple database testing solutions.
    /// </param>
    public TestingDatabaseBase(string Sign = "DB") {
        _database = DatabaseUtils.Q_Construct<TDatabase>(Sign);
    }

    [Fact]
    public void Migration() {
        IEnumerable<string> pendingMigrations = _database.Database.GetPendingMigrations();

        Assert.True(pendingMigrations.Empty(), $"Database instance isn't up-to-date with current database migrations. ({pendingMigrations.Count()} pendent)");
    }

    [Fact]
    public void Communication() {
        Assert.True(_database.Database.CanConnect(), $"{GetType()} cannot connect, check your connection credentials.");
    }

    [Fact]
    public void Evaluate() {
        _database.Evaluate();
    }
}
