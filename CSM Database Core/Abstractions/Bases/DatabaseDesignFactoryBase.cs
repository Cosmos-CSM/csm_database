using CSM_Database_Core.Abstractions.Interfaces;

using CSM_Foundation_Core.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CSM_Database_Core.Abstractions.Bases;


public class DatabaseDesignFactoryBase<TDatabase>
    : IDesignTimeDbContextFactory<TDatabase>
    where TDatabase : DbContext, IDatabase, new() {

    public virtual TDatabase CreateDbContext(string[] args) {
        ConsoleUtils.Warning(
            "Designing database using a design factory",
            new Dictionary<string, object?> {
                { "DesignFactory", GetType().FullName },
                { "Database", typeof(TDatabase).FullName  },
            }
        );

        return new TDatabase();
    }
}
