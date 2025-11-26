using System.ComponentModel.DataAnnotations;
using System.Reflection;

using CSM_Database_Core.Abstractions.Interfaces;
using CSM_Database_Core.Core.Errors;
using CSM_Database_Core.Core.Models;
using CSM_Database_Core.Core.Utilitites;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Bases;
using CSM_Foundation_Core.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSM_Database_Core;

/// <summary>
///     Represents a CSM Database, wich configures and handles data management along business entities and
///     data persistence.
/// </summary>
/// <typeparam name="TDatabases">
///     <see cref="DbContext"/> implementation of the database handler.
/// </typeparam>
public abstract partial class DatabaseBase<TDatabases>
    : DbContext, IDatabase
    where TDatabases : DbContext {

    public virtual string Sign { get; private set; } = "DB";

    /// <summary>
    ///     Whether the database context has logs enabled at building time.
    /// </summary>
    readonly bool _logsOn = true;

    /// <summary>
    ///     ORM Connection data.
    /// </summary>
    protected readonly ConnectionOptions _connection;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    /// <remarks> 
    ///     Connection configuration will be gathered from the <see cref="Sign"/> signature value config file.
    /// </remarks>
    public DatabaseBase(bool logsOn = true)
        : base() {

        _logsOn = logsOn;
        _connection = DatabaseUtils.Retrieve(Sign);
    }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="connectionOptions">
    ///     Database connection options.
    /// </param>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    /// <remarks> 
    ///     Connection configuration will be gathered from the <see cref="Sign"/> signature value config file.
    /// </remarks>
    public DatabaseBase(ConnectionOptions connectionOptions, bool logsOn = true)
        : base() {
        _logsOn = logsOn;
        _connection = connectionOptions;
    }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="dbOptions">
    ///     Native EntityFrameworkCore <see cref="DbContext"/> implementation options.
    /// </param>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    /// <remarks> 
    ///     Connection configuration will be gathered from the <see cref="Sign"/> signature value config file.
    /// </remarks>
    public DatabaseBase(DbContextOptions<TDatabases> dbOptions, bool logsOn = true)
        : base(dbOptions) {

        _logsOn = logsOn;
        _connection = DatabaseUtils.Retrieve(Sign);
    }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="connectionOptions">
    ///     Database connection options.
    /// </param>
    /// <param name="dbOptions">
    ///     Native EntityFrameworkCore <see cref="DbContext"/> implementation options.
    /// </param>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    /// <remarks> 
    ///     Connection configuration will be gathered from the <see cref="Sign"/> signature value config file.
    /// </remarks>
    public DatabaseBase(ConnectionOptions connectionOptions, DbContextOptions<TDatabases> dbOptions, bool logsOn = true)
        : base(dbOptions) {
        _logsOn = logsOn;
        _connection = connectionOptions;
    }

    /// <summary>
    ///     Generates a <see cref="DatabaseBase{TDatabases}"/> instance that handles specific database connection
    ///     and configuration properties/methods. 
    /// </summary>
    /// <param name="sign">
    ///     Database implementation signature to identify.
    /// </param>
    /// <remarks> 
    ///     This method gathers the <see cref="_connection"/> options from ./<see cref="Sign"/>(Upper)>/*.json files automatically.
    /// </remarks>
    public DatabaseBase([StringLength(5, MinimumLength = 5)] string sign)
        : base() {

        Sign = sign;
        _connection = DatabaseUtils.Retrieve(Sign);
    }

    /// <summary>
    ///     Generates a <see cref="DatabaseBase{TDatabases}"/> instance that handles specific database connection
    ///     and configuration properties/methods. 
    /// </summary>
    /// <param name="sign">
    ///     Database implementation signature to identify.
    /// </param>
    /// <param name="connection">
    ///     Database connection options.
    /// </param>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    public DatabaseBase([StringLength(5, MinimumLength = 5)] string sign, ConnectionOptions connection, bool logsOn = true)
        : base() {

        Sign = sign;
        _connection = connection;
        _logsOn = logsOn;
    }

    /// <summary>
    ///     Generates a <see cref="DatabaseBase{TDatabases}"/> instance that handles specific database connection
    ///     and configuration properties/methods. 
    /// </summary>
    /// <param name="sign">
    ///     Database implementation signature to identify
    /// </param>
    /// <param name="dbOptions">
    ///     Native EntityFrameworkCore <see cref="DbContext"/> implementation options.
    /// </param>
    /// <remarks> 
    ///     This method gathers the <see cref="_connection"/> options from ./<see cref="Sign"/>(Upper)>/*.json files automatically.
    /// </remarks>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    public DatabaseBase([StringLength(5, MinimumLength = 5)] string sign, DbContextOptions<TDatabases> dbOptions, bool logsOn = true)
        : base(dbOptions) {

        Sign = sign;
        _connection = DatabaseUtils.Retrieve(sign);
        _logsOn = logsOn;
    }

    /// <summary>
    ///     Generates a <see cref="DatabaseBase{TDatabases}"/> instance that handles specific database connection
    ///     and configuration properties/methods. 
    /// </summary>
    /// <param name="sign">
    ///     Database implementation signature to identify
    /// </param>
    /// <param name="connection">
    ///     Database connection options.
    /// </param>
    /// <param name="dbOptions">
    ///     Native EntityFrameworkCore <see cref="DbContext"/> implementation options.
    /// </param>
    /// <param name="logsOn">
    ///     Whether the logging service is enabled.
    /// </param>
    public DatabaseBase([StringLength(5, MinimumLength = 5)] string sign, ConnectionOptions connection, DbContextOptions<TDatabases> dbOptions, bool logsOn = true)
        : base(dbOptions) {

        Sign = sign;
        _connection = connection;
        _logsOn = logsOn;
    }

    /// <summary>
    ///     Validates if all the <see cref="Sets"/> <see cref="Type"/>s are <see cref="EntityBase"/> assuring contains the correct
    ///     methods needed.
    /// </summary>
    /// <returns>
    ///     The strict validated collection of [<see cref="BBusinessDatabaseEntity"/>]s and [<see cref="BConnector{TSource, TTarget}"/>]s.
    /// </returns>
    protected EntityBase[] ValidateSets() {
        Type databaseType = GetType();

        List<EntityBase> entityModels = [];
        IEnumerable<PropertyInfo> dbSets = databaseType
           .GetProperties()
           .Where(
               (propInfo) => {
                   Type propType = propInfo.PropertyType;

                   return propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(DbSet<>);
               }
           );

        foreach (PropertyInfo dbSet in dbSets) {
            Type generic = dbSet.PropertyType.GetGenericArguments()[0]
                ?? throw new DatabaseError(
                        DatabaseErrorEvents.WRONG_DBSET_ENTITY,
                        new Dictionary<string, object?> {
                                { "DbSet", dbSet.Name }
                            }
                    );

            if (!generic.IsInterface) {
                EntityBase instance = (EntityBase)Activator.CreateInstance(generic)!;
                entityModels.Add(instance);
                continue;
            }
        }

        return [.. entityModels];
    }


    public void ValidateHealth() {
        if (_logsOn) {
            ConsoleUtils.Announce(
                    $"Setting up ORM",
                    new() {
                                { "Database", GetType()?.Namespace ?? "---" },
                                { "Base", nameof(DatabaseBase<TDatabases>) }
                    }
                );
        }

        if (Database.CanConnect()) {

            if (_logsOn)
                ConsoleUtils.Success($"[{GetType().FullName}] ORM Set");

            IEnumerable<string> pendingMigrations = Database.GetPendingMigrations();
            if (pendingMigrations.Any()) {
                throw new Exception($"ORM ({GetType().FullName}) has pending migrations ({pendingMigrations.Count()})");
            }
            Evaluate();
        } else {
            try {
                Database.OpenConnection();
            } catch (Exception ex) {
                throw new Exception($"Invalid connection with Database ({GetType().FullName}) | {ex.InnerException?.Message}");
            }
        }
    }

    /// <summary>
    ///     Evaluates if <see cref="Sets"/> are correctly configured and translated to the internal framework handler.
    /// </summary>
    public void Evaluate() {
        EntityBase[] sets = ValidateSets();

        if (_logsOn) {
            ConsoleUtils.Announce(
                $"[{GetType().Name}] Validatig Sets...",
                new() {
                { "Count", sets.Length }
                }
            );
        }

        Exception[] evResults = [];
        foreach (EntityBase set in sets) {
            Exception[] result = set.EvaluateDefinition();
            if (result.Length > 0 && _logsOn) {
                ConsoleUtils.Warning(
                    "Wrong [Set] definition",
                    new() {
                        { "Set", set.GetType().Name },
                        { "Exceptions", result },
                    }
                );
            }

            evResults = [.. evResults, .. result];
        }

        if (evResults.Length > 0) {
            throw new Exception("Database [Set] definition failures");
        } else if (_logsOn) {
            ConsoleUtils.Success($"[{GetType().Name}] Set validation succeeded");
        }
    }

    protected virtual void DesignEntity(EntityBase Entity, EntityTypeBuilder mBuilder) { }

    protected virtual void DesignDb(ModelBuilder mBuilder) { }

    #region EF Native Methods


    /// <summary>
    ///     This is overriden from <see cref="DatabaseBase{TDatabases}"/> to Configure an SQL Server Connection using
    ///     <see cref="_connection"/> generated string, this natively has another behavior but using <see cref="DatabaseBase{TDatabases}"/>
    ///     will automatically configure the SQL Server connection.
    /// </summary>
    /// <param name="optionsBuilder">
    ///     Relations builder proxy object.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string connectionString = _connection.GenerateConnectionString();
        optionsBuilder.UseSqlServer(connectionString);

        /// We catch when the execution context is an Entity Framework design runtime.
        if (AppDomain.CurrentDomain.FriendlyName.Contains("ef")) {

            string envValue = SystemUtils.GetVar("ASPNETCORE_ENVIRONMENT") ?? SystemUtils.GetVar("DOTNET_ENVIRONMENT") ?? "---";

            if (_logsOn) {
                ConsoleUtils.Warning(
                        $"Running EF Design Time Execution",
                        new Dictionary<string, object?> {
                        { "Environment", envValue },
                        { "Connection", connectionString },
                        }
                    );
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder mBuilder) {

        DesignDb(mBuilder);

        IEnumerable<IMutableEntityType> entityTypes = mBuilder.Model.GetEntityTypes();
        foreach (IMutableEntityType entityType in entityTypes) {

            IEnumerable<IMutableForeignKey> foreignKeys = [.. entityType.GetForeignKeys()];
            foreach (IMutableForeignKey foreignKey in foreignKeys) {

                if (foreignKey.DependentToPrincipal is null) {
                    continue;
                }

                mBuilder.Entity(entityType.ClrType).Ignore(foreignKey.DependentToPrincipal.Name);
            }
        }

        EntityBase[] sets = ValidateSets();

        foreach (EntityBase entity in sets) {
            Type setType = entity.GetType();
            mBuilder.Entity(
                setType,
                (etBuilder) => {
                    etBuilder.HasKey(nameof(IEntity.Id));
                    etBuilder.Property<long>(nameof(IEntity.Id)).IsRequired();

                    if (entity is INamedEntity) {
                        PropertyInfo nameProperty = entity.GetProperty(nameof(INamedEntity.Name));
                        PropertyInfo descriptionProperty = entity.GetProperty(nameof(INamedEntity.Description));

                        etBuilder.HasIndex(nameProperty.Name).IsUnique();
                        etBuilder.Property(nameProperty.Name).HasMaxLength(100).IsRequired();

                        etBuilder.Property(descriptionProperty.Name).HasMaxLength(200);
                    }

                    if (entity is IReferencedEntity) {
                        PropertyInfo referenceProperty = entity.GetProperty(nameof(IReferencedEntity.Reference));

                        etBuilder.HasIndex(referenceProperty.Name)
                            .IsUnique();

                        etBuilder.Property(referenceProperty.Name)
                            .HasMaxLength(8)
                            .IsFixedLength()
                            .IsRequired();
                    }

                    if (entity is IActivableEntity) {
                        PropertyInfo isEnabledProperty = entity.GetProperty(nameof(IActivableEntity.IsEnabled));

                        etBuilder.Property(isEnabledProperty.Name)
                            .IsRequired();
                    }

                    DesignEntity(entity, etBuilder);

                    etBuilder.Property(nameof(IEntity.Timestamp))
                        .HasColumnType("datetime2(7)")
                        .HasDefaultValueSql("GETUTCDATE()");

                    entity.DesignEntity(etBuilder);
                }
            );
        }

        base.OnModelCreating(mBuilder);
    }

    public bool Validate(bool strict = true) {
        throw new NotImplementedException();
    }

    #endregion
}

/// <inheritdoc cref="Entities.Abstractions.Bases.BEntity"/>
public abstract partial class EntityBase
    : ObjectBase<IEntity>, IEntity {

    /// <summary>
    ///     Describe to the Entity Framework manager how to handle the [Entity] object, its proeprties and relations, instructing
    ///     the <see cref="EntityTypeBuilder"/> how to handle them.
    /// </summary>
    /// <param name="etBuilder">
    ///     Proxy object to configure Entity Model to Entity Framework Core.
    /// </param>
    /// <remarks>
    ///     Don't describe <see cref="IEntity"/> properties they are being auto-described by the [CSM] engine, <see cref="IEntity.Id"/>, <see cref="IEntity.Timestamp"/> and <see cref="IEntity.Name"/>.
    /// </remarks>
    protected internal virtual void DesignEntity(EntityTypeBuilder etBuilder) { }
}