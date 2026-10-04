using CSM_Foundation_Core.Core.Utils;

using Microsoft.EntityFrameworkCore.Design;

namespace CSM_Security_Database_Core;

/// <summary>
///     EF Design time factory for <see cref="DatabaseProxy.DatabaseProxy"/>
/// </summary>
internal class DatabaseProxyDesignFactory
    : IDesignTimeDbContextFactory<DatabaseProxy.DatabaseProxy> {


    public DatabaseProxy.DatabaseProxy CreateDbContext(string[] args) {
        ConsoleUtils.Warning(
            "Designing database using a design factory",
            new Dictionary<string, object?> {
                { "DesignFactory", GetType().FullName },
                { "Database", typeof(DatabaseProxy.DatabaseProxy).FullName  },
            }
        );

        return new DatabaseProxy.DatabaseProxy();
    }
}
