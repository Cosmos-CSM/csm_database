using CSM_Foundation.Database;

using CSM_Foundation_Database.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace CSM_Foundation_Database.UnitTests;

/// <summary>
///     Represents an <see cref="IEntity"/> with an activator reference mock.
/// </summary>
[ActivatorReference(typeof(EntityMock_ReferencedInterface))]
interface IReferencedEntity
    : IEntity {

}

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
    : BEntity {

    public override Type Database { get; init; } = typeof(DbContext);
}

/// <summary>
///     Represents a <see cref="BDatabase{TDatabases}"/> implementation used as an internal mock
///     for unit testing, with referenced interface scenario.
/// </summary>
class BDatabaseMock_ReferencedInterface
    : BDatabase<DbContext> {


    public DbSet<IReferencedEntity> EntityMock { get; set; } = default!;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public BDatabaseMock_ReferencedInterface()
        : base(
                "CSMST",
                new Models.ConnectionOptions {
                    Host = "",
                    Name = "",
                    User = "",
                    Password = "",
                }
            ) {
    }


    public new BEntity[] ValidateSets() {
        return base.ValidateSets();
    }
}

/// <summary>
///     Represents a <see cref="BDatabase{TDatabases}"/> implementation used as an internal mock
///     for unit testing, with unreferenced interface scenario.
/// </summary>
class BDatabaseMock_UnreferencedInterface
    : BDatabase<DbContext> {


    public DbSet<IUnrefrencedEntity> EntityMock { get; set; } = default!;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public BDatabaseMock_UnreferencedInterface()
        : base(
                "CSMST",
                new Models.ConnectionOptions {
                    Host = "",
                    Name = "",
                    User = "",
                    Password = "",
                }
            ) {
    }


    public new BEntity[] ValidateSets() {
        return base.ValidateSets();
    }
}

/// <summary>
///     Represents a tests class for <see cref="BDatabase{TDatabases}"/> base implementation.
/// </summary>
public class BDatabase_UnitTests {


    [Fact(DisplayName = "[ValidateSets]: Activation success with referenced interface")]
    public void ValidateSetsA() {
        BDatabaseMock_ReferencedInterface mock = new();

        BEntity[] contextEntities = mock.ValidateSets();

        Assert.Single(contextEntities);
    }

    [Fact(DisplayName = "[ValidateSets]: Activation fails with not referenced interface")]
    public void ValidateSetsB() {
        BDatabaseMock_UnreferencedInterface mock = new();

        XDatabase exception = Assert.Throws<XDatabase>(mock.ValidateSets);
        Assert.Equal(XDatabaseEvents.INTERFACE_UNCONFIGURED, exception.Event);
    }
}
