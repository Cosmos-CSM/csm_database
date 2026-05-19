using CSM_Database_Core;
using CSM_Database_Core.Core.Models;

using DatabaseProxy.Entities;

using Microsoft.EntityFrameworkCore;

namespace DatabaseProxy;

/// <summary>
///     Represents a proxy database implementation.
/// </summary>
public class DatabaseProxy
    : DatabaseBase<DatabaseProxy> {

    /// <inheritdoc/>
    public override string Sign { get; protected set; } = "CSMDQ";

    /// <inheritdoc/>
    public DatabaseProxy()
        : base() {

    }

    /// <inheritdoc/>
    public DatabaseProxy(DatabaseOptions<DatabaseProxy> options)
        : base(options) {
    }

    /// <summary>
    ///     [EntityProxy] table set.
    /// </summary>
    public DbSet<EntityProxy> EntityProxies { get; set; } = default!;

    /// <summary>
    ///     [EntityDependantProxy] table set.
    /// </summary>
    public DbSet<EntityDependantProxy> EntityDependantProxies { get; set; } = default!;

    /// <summary>
    ///     [EntityDependencyProxy] table set.
    /// </summary>
    public DbSet<EntityDependencyProxy> EntityDependencyProxies { get; set; } = default!;
}
