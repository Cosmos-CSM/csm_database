using System.Reflection;

using CSharp_Database_Extension.Abstractions.Interfaces;
using CSharp_Database_Extension.Common.Enums;
using CSharp_Database_Extension.Common.Errors;
using CSharp_Database_Extension.Common.Models;
using CSharp_Database_Extension.Common.Utils;
using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

using CSharp_Extension.Common.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSharp_Database_Extension;

/// <summary>
///     Represents a Database Model.
/// </summary>
/// <typeparam name="TDatabases">
///     Type of the database model.
/// </typeparam>
public abstract partial class DatabaseBase<TDatabases>
    : DbContext, IDatabase
    where TDatabases : DbContext {

    /// <inheritdoc/>
    public virtual string Signature { get; protected set; } = "";

    /// <inheritdoc/>
    public virtual DatabaseProviders Provider { get; protected set; } = DatabaseProviders.PostgreSQL;

    /// <summary>
    ///     Database model options.
    /// </summary>
    public DatabaseOptions<TDatabases> Options { get; init; }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public DatabaseBase() {
        Options = new DatabaseOptions<TDatabases>();
        CompleteOptions();
    }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="databaseOptions">
    ///     EF Database context options.
    /// </param>
    public DatabaseBase(DatabaseOptions<TDatabases> databaseOptions)
        : base(databaseOptions.DbContextOptions ?? new()) {
        Options = databaseOptions;

        CompleteOptions();
    }

    /// <inheritdoc/>
    public bool Validate(bool strict = true) {
        bool logsOn = Options.EnableLogging;

        if (logsOn) {
            ConsoleUtils.Announce(
                    $"Setting up ORM",
                    new() {
                        { "Database", GetType()?.Namespace ?? "---" },
                        { "Base", nameof(DatabaseBase<>) }
                    }
                );
        }

        if (Database.CanConnect()) {
            if (logsOn)
                ConsoleUtils.Success($"[{GetType().FullName}] ORM Set");

            IEnumerable<string> pendingMigrations = Database.GetPendingMigrations();
            if (pendingMigrations.Any()) {

                if (strict)
                    throw new Exception($"ORM ({GetType().FullName}) has pending migrations ({pendingMigrations.Count()})");

                return false;
            }
            return ValidateEntityModels(strict);
        }

        try {
            Database.OpenConnection();
            return true;
        } catch (Exception ex) {

            if (strict)
                throw new Exception($"Invalid connection with Database ({GetType().FullName}) | {ex.InnerException?.Message}");

            return false;
        }
    }

    /// <summary>
    ///     Gets database entity models.
    /// </summary>
    /// <returns>
    ///     Database entity instances as models.
    /// </returns>
    protected EntityBase[] GetEntityModels() {
        Type dbType = GetType();

        List<EntityBase> entityModels = [];
        IEnumerable<PropertyInfo> dbSets = dbType
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

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string connectionString = Options.ConnectionOptions!.ConnectionString;

        string assemblyNamespace = $"{GetType().Assembly.GetName().Name}.Migrations";

        switch (Options.Provider) {
            case DatabaseProviders.SQLServer:
                optionsBuilder.UseSqlServer(
                        connectionString,
                        builder => builder.MigrationsAssembly($"{assemblyNamespace}.SQLServer")
                    );
                break;
            case DatabaseProviders.PostgreSQL:
                optionsBuilder.UseNpgsql(
                        connectionString,
                        builder => builder.MigrationsAssembly($"{assemblyNamespace}.PostgreSQL")
                    );
                break;
        }

        // We catch when the execution context is an Entity Framework design runtime.
        if (AppDomain.CurrentDomain.FriendlyName.Contains("ef")) {

            string envValue = SystemUtils.GetVar("ASPNETCORE_ENVIRONMENT") ?? SystemUtils.GetVar("DOTNET_ENVIRONMENT") ?? "---";

            if (Options.EnableLogging) {
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

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder mBuilder) {

        DesignDatabase(mBuilder);

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

        EntityBase[] entityModels = GetEntityModels();

        foreach (EntityBase entity in entityModels) {
            Type entityModelType = entity.GetType();
            mBuilder.Entity(
                entityModelType,
                (etBuilder) => {
                    DesignEntityModelInterfaces(etBuilder, entity);
                    DesignEntity(entity, etBuilder);

                    entity.DesignEntity(etBuilder);
                }
            );
        }

        base.OnModelCreating(mBuilder);
    }

    /// <summary>
    ///     Designs the database entity models type, in order to be translated to database provider language.
    /// </summary>
    /// <param name="entity">
    ///     Entity Model to be designed.
    /// </param>
    /// <param name="mBuilder">
    ///     Global database entity model builder.
    /// </param>
    protected virtual void DesignEntity(EntityBase entity, EntityTypeBuilder mBuilder) { }

    /// <summary>
    ///     Designs the current <paramref name="mBuilder"/> instance for the database, overriding the current behavior.
    /// </summary>
    /// <param name="mBuilder">
    ///     Global database model builder instance.
    /// </param>
    protected virtual void DesignDatabase(ModelBuilder mBuilder) { }

    /// <summary>
    ///     Completes missing <see cref="Options"/> values that are auto-loaded
    ///     by system properties or values calculated at runtime.
    /// </summary>
    /// <remarks>
    ///     Important process, is required to be called in each constructor of this class.
    /// </remarks>
    void CompleteOptions() {
        Options.Signature ??= Signature;

        // When current options provider is the default one.
        if (Options.Provider == DatabaseProviders.PostgreSQL) {
            Options.Provider = Provider;
        }

        Options.ConnectionOptions ??= DatabaseUtils.GetConnectionOptions(Signature, Options.ForTesting);
    }

    /// <summary>
    ///     Validate database entity models.
    /// </summary>
    /// <param name="strict">
    ///     Whether the engines should be stopped on invalid entities.
    /// </param>
    bool ValidateEntityModels(bool strict = true) {

        bool logsOn = Options.EnableLogging;
        EntityBase[] defs = GetEntityModels();

        if (logsOn) {
            ConsoleUtils.Announce(
                $"[{GetType().Name}] Validatig Entity Models...",
                new() {
                    { "Count", defs.Length }
                }
            );
        }

        Exception[] evResults = [];
        foreach (EntityBase entity in defs) {
            Exception[] result = entity.EvaluateModel();
            if (result.Length > 0 && logsOn) {
                ConsoleUtils.Warning(
                    "Wrong [Entity Model] definition",
                    new() {
                        { "Entity Model", entity.GetType().Name },
                        { "Result", result },
                    }
                );
            }

            evResults = [.. evResults, .. result];
        }

        if (evResults.Length > 0) {
            if (strict)
                throw new Exception("Database [Entity Models] validation failed");

            return false;
        }

        if (logsOn)
            ConsoleUtils.Success($"[{GetType().Name}] Set validation succeeded");

        return true;
    }

    /// <summary>
    ///     Design business interfacing for business entities with their defined properties and behavior.
    /// </summary>
    /// <param name="etBuilder">
    ///     Entity model builder.
    /// </param>
    /// <param name="entity">
    ///     Entity instance modeled.
    /// </param>
    static void DesignEntityModelInterfaces(EntityTypeBuilder etBuilder, EntityBase entity) {
        etBuilder.HasKey(nameof(IEntity.Id));
        etBuilder.Property<long>(nameof(IEntity.Id))
            .IsRequired();

        PropertyInfo nameProperty = entity.GetProperty(nameof(IEntity.Name));
        PropertyInfo descriptionProperty = entity.GetProperty(nameof(IEntity.Description));

        etBuilder
            .HasIndex(nameProperty.Name)
            .IsUnique();
        etBuilder
            .Property(nameProperty.Name)
            .HasMaxLength(200)
            .IsRequired();

        etBuilder
            .Property(descriptionProperty.Name);

        etBuilder.Property(nameof(IEntity.Timestamp))
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRowVersion();
    }
}

/// <inheritdoc cref="EntityBase"/>
public abstract partial class EntityBase {

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