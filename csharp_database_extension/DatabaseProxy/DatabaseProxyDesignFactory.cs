using CSharp_Extension.Common.Utils;

using Microsoft.EntityFrameworkCore.Design;

namespace DatabaseProxy;

/// <summary>
///     EF Design time factory for <see cref="DatabaseProxy.DatabaseProxy"/>
/// </summary>
internal class DatabaseProxyDesignFactory
    : IDesignTimeDbContextFactory<DatabaseProxy> {


    public DatabaseProxy CreateDbContext(string[] args) {
        ConsoleUtils.Warning(
            "Designing database using a design factory",
            new Dictionary<string, object?> {
                { "DesignFactory", GetType().FullName },
                { "Database", typeof(DatabaseProxy).FullName  },
            }
        );

        return new DatabaseProxy();
    }
}
