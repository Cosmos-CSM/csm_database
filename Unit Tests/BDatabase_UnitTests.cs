using CSM_Database_Core;
using CSM_Database_Core.Core.Errors;
using CSM_Database_Core.Core.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace UnitTests;

/// <summary>
///     Represents an <see cref="IEntity"/> with unreferenced entity mock.
/// </summary>
interface IUnrefrencedEntity
    : IEntity {

}

/// <summary>
///     Represents an <see cref="IEntity"/> mock implementation for a referenced interface scenario.
/// </summary>
class EntityMock_ReferencedInterface
    : EntityBase {

    public override Type Database { get; init; } = typeof(DbContext);
}

/// <summary>
///     Represents a <see cref="DatabaseBase{TDatabases}"/> implementation used as an internal mock
///     for unit testing, with referenced interface scenario.
/// </summary>
class BDatabaseMock_ReferencedInterface
    : DatabaseBase<DbContext> {


    public DbSet<IReferencedEntity> EntityMock { get; set; } = default!;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public BDatabaseMock_ReferencedInterface()
        : base(
                "CSMST",
                new ConnectionOptions {
                    Host = "",
                    Name = "",
                    User = "",
                    Password = "",
                }
            ) {
    }


    public new EntityBase[] ValidateSets() {
        return base.ValidateSets();
    }
}

/// <summary>
///     Represents a <see cref="DatabaseBase{TDatabases}"/> implementation used as an internal mock
///     for unit testing, with unreferenced interface scenario.
/// </summary>
class BDatabaseMock_UnreferencedInterface
    : DatabaseBase<DbContext> {


    public DbSet<IUnrefrencedEntity> EntityMock { get; set; } = default!;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public BDatabaseMock_UnreferencedInterface()
        : base(
                "CSMST",
                new ConnectionOptions {
                    Host = "",
                    Name = "",
                    User = "",
                    Password = "",
                }
            ) {
    }

    public new CSM_Database_Core.EntityBase[] ValidateSets() {
        return base.ValidateSets();
    }
}

/// <summary>
///     Represents a tests class for <see cref="DatabaseBase{TDatabases}"/> base implementation.
/// </summary>
public class BDatabase_UnitTests {
    [Fact(DisplayName = "[ValidateSets]: Activation success with referenced interface")]
    public void ValidateSetsA() {
        BDatabaseMock_ReferencedInterface mock = new();

        CSM_Database_Core.EntityBase[] contextEntities = mock.ValidateSets();

        Assert.Single(contextEntities);
    }

    [Fact(DisplayName = "[ValidateSets]: Activation fails with not referenced interface")]
    public void ValidateSetsB() {
        BDatabaseMock_UnreferencedInterface mock = new();

        DatabaseError exception = Assert.Throws<DatabaseError>(mock.ValidateSets);
        Assert.Equal(DatabaseErrorEvents.INTERFACE_UNCONFIGURED, exception.Event);
    }
}
