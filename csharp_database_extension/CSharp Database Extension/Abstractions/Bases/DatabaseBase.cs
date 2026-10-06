using System.Reflection;

using CSharp_Database_Extension.Abstractions.Interfaces;
using CSharp_Database_Extension.Common.Models;
using CSharp_Database_Extension.Core.Errors;
using CSharp_Database_Extension.Core.Utils;
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

    /// <summary>
    ///     Database model; options.
    /// </summary>
    public DatabaseOptions<TDatabases> Options { get; init; }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public DatabaseBase() {
        Options = new DatabaseOptions<TDatabases>();
        BuildOptions();
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

        BuildOptions();
    }


    #region Public

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
            return ValidateEntityDefinitions(strict);
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

    #endregion

    /// <summary>
    ///     Validates <see cref="Options"/> dependencies and generates required ones.
    /// </summary>
    /// <remarks>
    ///     Important process, is required to be called in each constructor.
    /// </remarks>
    void BuildOptions() {
        Options.Signature ??= Signature;

        Options.ConnectionOptions ??= DatabaseUtils.GetConnectionOptions(Signature, Options.ForTesting);
    }

    /// <summary>
    ///     Gets all database entity definitions.
    /// </summary>
    /// <returns>
    ///     Database entity definitions.
    /// </returns>
    protected EntityBase[] GetEntityDefinitions() {
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

    /// <summary>
    ///     Validate that all database entities are correctly defined.
    /// </summary>
    /// <param name="strict">
    ///     Whether the engines should be stopped on invalid entities.
    /// </param>
    bool ValidateEntityDefinitions(bool strict = true) {

        bool logsOn = Options.EnableLogging;
        EntityBase[] defs = GetEntityDefinitions();

        if (logsOn) {
            ConsoleUtils.Announce(
                $"[{GetType().Name}] Validatig Sets...",
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
                    "Wrong [DbSet] definition",
                    new() {
                        { "Set", entity.GetType().Name },
                        { "Exceptions", result },
                    }
                );
            }

            evResults = [.. evResults, .. result];
        }

        if (evResults.Length > 0) {
            if (strict)
                throw new Exception("Database [DbSet] definition failures");

            return false;
        }

        if (logsOn)
            ConsoleUtils.Success($"[{GetType().Name}] Set validation succeeded");

        return true;
    }

    /// <summary>
    ///     Designs the current <paramref name="mBuilder"/> instance for the given <paramref name="entity"/>, overriding
    ///     the current behavior.
    /// </summary>
    /// <param name="entity">
    ///     Entity instance being designed.
    /// </param>
    /// <param name="mBuilder">
    ///     Global database model builder instance.
    /// </param>
    protected virtual void DesignEntity(EntityBase entity, EntityTypeBuilder mBuilder) { }

    /// <summary>
    ///     Designs the current <paramref name="mBuilder"/> instance for the database, overriding the current behavior.
    /// </summary>
    /// <param name="mBuilder">
    ///     Global database model builder instance.
    /// </param>
    protected virtual void DesignDatabase(ModelBuilder mBuilder) { }

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string connectionString = Options.ConnectionOptions!.ConnectionString;
        optionsBuilder.UseSqlServer(connectionString);

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

        EntityBase[] sets = GetEntityDefinitions();

        foreach (EntityBase entity in sets) {
            Type setType = entity.GetType();
            mBuilder.Entity(
                setType,
                (etBuilder) => {
                    etBuilder.HasKey(nameof(IEntity.Id));
                    etBuilder.Property<long>(nameof(IEntity.Id)).IsRequired();

                    DesignEntityInterfacing(etBuilder, entity);
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

    /// <summary>
    ///     Design business interfacing for business entities with their defined properties and behavior.
    /// </summary>
    /// <param name="etBuilder">
    ///     Entity model builder.
    /// </param>
    /// <param name="entity">
    ///     Entity instance modeled.
    /// </param>
    static void DesignEntityInterfacing(EntityTypeBuilder etBuilder, EntityBase entity) {
        if (entity is INamedEntity) {
            PropertyInfo nameProperty = entity.GetProperty(nameof(INamedEntity.Name));
            PropertyInfo descriptionProperty = entity.GetProperty(nameof(INamedEntity.Description));

            etBuilder
                .HasIndex(nameProperty.Name)
                .IsUnique();
            etBuilder
                .Property(nameProperty.Name)
                .HasMaxLength(100).IsRequired();

            etBuilder
                .Property(descriptionProperty.Name);
        }

        if (entity is IActivableEntity) {
            PropertyInfo isEnabledProperty = entity.GetProperty(nameof(IActivableEntity.IsEnabled));

            etBuilder.Property(isEnabledProperty.Name)
                .IsRequired();
        }
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
    ///     Don't describe <see cref="IEntity"/> properties they are being auto-described by the [CSM] engine, <see cref="IEntity.Id"/>, <see cref="IEntity.Timestamp"/> and <see cref="INamedEntity.Name"/>.
    /// </remarks>
    protected internal virtual void DesignEntity(EntityTypeBuilder etBuilder) { }
}