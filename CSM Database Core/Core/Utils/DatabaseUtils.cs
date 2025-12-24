using System.Reflection;
using System.Text.Json;

using CSM_Database_Core.Core.Attributes;
using CSM_Database_Core.Core.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core;
using CSM_Foundation_Core.Core.Exceptions;
using CSM_Foundation_Core.Core.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CSM_Database_Core.Core.Utils;

public class DatabaseUtils {
    const string DirectoryName = ".Connection";
    const string QualityPrefix = "quality";
    const string DevelopmentPrefix = "development";
    const string ProductionPrefix = "production";


    /// <summary>
    ///     Connection file name template for Quality environment variable.
    /// </summary>
    const string Q_CONNTION_TMPLATE = "Q_{0}.Connection";

    /// <summary>
    ///     Gets the database context connection options for testing instance.
    /// </summary>
    /// <param name="sign">
    ///     Database context signature.
    /// </param>
    /// <returns>
    ///     Testing purposes database context connection options.
    /// </returns>
    /// <exception cref="Exception">
    /// </exception>
    static ConnectionOptions GetTestingConnectionOptions(string sign) {
        string connVar = string.Format(Q_CONNTION_TMPLATE, sign);

        string connPath = Environment.GetEnvironmentVariable(connVar)
            ?? throw new Exception($" Testing connection options path variable not found for ({sign}) (Make sure the environment variable [{connVar}] is set at the .runsettings tests context file)");

        using FileStream fileReader = new(connPath, FileMode.Open, FileAccess.Read);

        ConnectionOptions connectionOptions = JsonSerializer.Deserialize<ConnectionOptions>(fileReader)
            ?? throw new Exception($"File ({connPath}) doesn't contain the correct format for ({nameof(ConnectionOptions)}) model");

        return connectionOptions;
    }

    /// <summary>
    ///     Gets the database context connection options.
    /// </summary>
    /// <param name="sign">
    ///     Database context signature.
    /// </param>
    /// <param name="forTesting">
    ///     Whether the build process must fetch the connection options for testing purposes.
    /// </param>
    /// <returns>
    ///     Database context connection options.
    /// </returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="Exception"/>
    public static ConnectionOptions GetConnectionOptions(string sign, bool forTesting = false) {

        if (forTesting) {
            return GetTestingConnectionOptions(sign);
        }

        string envVarName = $"{sign}_design_connection".ToLower();
        string filePath = "";
        EnvironmentVariableTarget[] envVarTargetsSequence = [
                EnvironmentVariableTarget.Machine,
                EnvironmentVariableTarget.User,
                EnvironmentVariableTarget.Process,
            ];

        string? envVarValue = null;
        EnvironmentVariableTarget? usedEnvTarget = null;
        foreach (EnvironmentVariableTarget envTargetSeq in envVarTargetsSequence) {
            if (!string.IsNullOrWhiteSpace(filePath))
                break;

            envVarValue = SystemUtils.GetVar(envVarName, envTargetSeq);
            if (!string.IsNullOrWhiteSpace(envVarValue)) {
                filePath = envVarValue;
                usedEnvTarget = envTargetSeq;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(filePath)) {
            string appDir = AppContext.BaseDirectory;

            string envPrefix = SystemUtils.GetEnv() switch {
                SystemEnvs.DEV => Constants.Environments.DEV,
                SystemEnvs.PROD => Constants.Environments.PROD,
                SystemEnvs.QA => Constants.Environments.QA,
                SystemEnvs.LAB => Constants.Environments.LAB,
                _ => DevelopmentPrefix,
            };

            string fileName = $"{sign.ToLower()}.{envPrefix}.connection.json";
            string[] appDirFiles = Directory.GetFiles(appDir);
            string appDirConnFile = appDirFiles
                .Where(i => i == fileName)
                .FirstOrDefault()
                ?? throw new FileNotFoundException($"{appDir}\\{fileName} not in app assemblies");

            filePath = appDirConnFile;
        }

        if (string.IsNullOrWhiteSpace(filePath)) {
            ConsoleUtils.Error(
                    "Unable to locate the connection options file path",
                    details: new Dictionary<string, object?> {
                        { "Environment Variable Name", envVarName },
                        { "Environment Variable Value", envVarValue },
                        { "Environment Variable Target", usedEnvTarget },
                        { "App Directory", AppContext.BaseDirectory },
                        { "Signature", sign },
                    }
                );
            throw new ArgumentNullException(filePath);
        }

        using FileStream pfs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        ConnectionOptions? m = JsonSerializer.Deserialize<ConnectionOptions>(pfs);
        pfs.Dispose();

        return m is null ? throw new Exception() : m;
    }

    /// <summary>
    ///     Sanitizes the entity relations to ensure that the relations are correctly tracked from the database and avoid the creation of relation entities wrongly given through the main entity.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the Main Entity to be sanitized.
    /// </typeparam>
    /// <param name="database">
    ///     Database context handler for Entity to be sanitized.
    /// </param>
    /// <param name="entity">
    ///     Entity instance to be sanitized
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the dbSet couldn't be found for the relation entity.
    ///     Thrown when a relation Entity is being tried to be created automatically.
    ///     Thrown when the relation isn´t a IEntity implementation neither a Collection of IEntity.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown when the relation entity couldn't be found in the database.
    /// </exception>
    public static TEntity SanitizeEntity<TEntity>(DbContext database, TEntity entity) {

        IQueryable<IEntity> GetDbSet(Type entityType) {
            object objectDbSet = typeof(DbContext)
                .GetMethods()
                .Where(
                    m => m.Name == nameof(DbContext.Set) && m.IsGenericMethod && m.GetGenericArguments().Length == 1
                )
                .FirstOrDefault()?
                .MakeGenericMethod(entityType)
                .Invoke(database, null)
                ?? throw new($"DbContext({database.GetType().Name}) doesn´t have the neccesary DbSet({entityType.Name}) method", null);

            return (IQueryable<IEntity>)objectDbSet;
        }

        if (entity == null) {
            return entity;
        }

        IEnumerable<PropertyInfo> relationProperties = entity
            .GetType()
            .GetProperties()
            .Where(
                (pi) => pi.GetCustomAttribute<EntityRelationAttribute>() != null
            );

        foreach (PropertyInfo relationProperty in relationProperties) {
            Type relationType = relationProperty.PropertyType;
            object? relationValue = relationProperty.GetValue(entity);

            if (relationValue is null) {
                continue;
            }

            bool isCollection = relationValue is IEnumerable<IEntity>;
            bool isEntity = relationValue is IEntity;

            if (!isEntity && !isCollection) {
                throw new SystemError($"Entity relation integrity problem, relation has [Relation] attribute but is not a IEnumerable<IEntity> neither IEntity assignable relation", null);
            }

            if (isEntity) {
                IEntity relEntity = (IEntity)relationValue;

                if (relEntity.Id <= 0) {
                    throw new SystemError($"Dependencies aren't allowed to be created on main Entity creation", null);
                }

                IQueryable<IEntity> dbSet = GetDbSet(relEntity.GetType());

                IEntity dbRelEntity = dbSet.Where(
                        entity => entity.Id == relEntity.Id
                    )
                    .FirstOrDefault()
                    ?? throw new SystemError($"Couldn't find relation entity ({relEntity.GetType().Name})[{relEntity.Id}]", null);

                relationProperty.SetValue(entity, dbRelEntity);
                EntityEntry entityEntry = database.Entry(dbRelEntity);
                if (entityEntry.State == EntityState.Detached) {
                    entityEntry.State = EntityState.Unchanged;
                }
            } else {
                /// --> At this point we already know it's a collection relation.
                IEnumerable<IEntity> relCollection = (IEnumerable<IEntity>)relationValue;
                if (!relCollection.Any())
                    continue;

                IEnumerable<object> dbRelCollection = [];
                Type relEntityType = relCollection.First().GetType();
                IQueryable<IEntity> dbSet = GetDbSet(relEntityType);

                foreach (IEntity relEntity in relCollection) {

                    IEntity dbRelEntity = dbSet.Where(
                            entity => entity.Id == relEntity.Id
                        )
                        .FirstOrDefault()
                        ?? throw new SystemError($"Couldn't find relation entity ({relEntity.GetType().Name})[{relEntity.Id}]", null); ;

                    EntityEntry relEntityEntry = database.Entry(dbRelEntity);
                    if (relEntityEntry.State == EntityState.Detached) {
                        relEntityEntry.State = EntityState.Unchanged;
                    }

                    dbRelCollection = dbRelCollection.Append(dbRelEntity);
                }

                object castedCollection = typeof(Enumerable)
                    .GetMethod("Cast")?
                    .MakeGenericMethod(relEntityType)
                    .Invoke(null,
                        [
                            dbRelCollection
                        ]
                    )
                    ?? throw new SystemError($"Unable to cast IEntity to Entity type object", null);

                castedCollection = typeof(Enumerable)
                    .GetMethod("ToList")?
                    .MakeGenericMethod(relEntityType)
                    .Invoke(
                        null,
                        [
                            castedCollection
                        ]
                    )
                    ?? throw new SystemError("Unable to convert entity collection", null);

                relationProperty.SetValue(entity, castedCollection);
            }
        }
        return entity;
    }

    /// <summary>
    ///     Sanitizes an update entity operation.
    /// </summary>
    /// <param name="database">
    ///     Database context.
    /// </param>
    /// <param name="original"> 
    ///     Current stored entity data.
    /// </param>
    /// <param name="new">
    ///     New entity data to overwrite.
    /// </param>
    public static void SanitizeUpdateEntity(DbContext database, IEntity original, IEntity @new) {
        EntityEntry previousEntry = database.Entry(original);
        if (previousEntry.State == EntityState.Unchanged) {
            // Update the non-navigation properties.
            previousEntry.CurrentValues.SetValues(@new);
            foreach (NavigationEntry navigation in previousEntry.Navigations) {
                object? newNavigationValue = database.Entry(@new).Navigation(navigation.Metadata.Name).CurrentValue;
                // Validate if navigation is a collection.
                if (navigation.CurrentValue is IEnumerable<object> previousCollection && newNavigationValue is IEnumerable<object> newCollection) {
                    List<object> previousList = [.. previousCollection];
                    List<object> newList = [.. newCollection];
                    // Perform a search for new items to add in the collection.
                    // NOTE: the followings iterations must be performed in diferent code segments to avoid index length conflicts.
                    for (int i = 0; i < newList.Count; i++) {
                        IEntity? newItemSet = (IEntity)newList[i];
                        if (newItemSet != null && newItemSet.Id <= 0) {
                            // Getting the item type to add.
                            Type itemType = newItemSet.GetType();
                            // Getting the Add method from Icollection.
                            MethodInfo? addMethod = previousCollection.GetType().GetMethod("Add", [itemType]);
                            // Adding the new item to Icollection.
                            _ = (addMethod?.Invoke(previousCollection, [newItemSet]));

                        }
                    }
                    // Find items to modify.
                    for (int i = 0; i < previousList.Count; i++) {
                        // For each new item stored in overwritten collection, will search for an ID match and update the overwritten.
                        foreach (object newitem in newList) {
                            if (previousList[i] is IEntity previousItem && newitem is IEntity newItemSet && previousItem.Id == newItemSet.Id) {
                                SanitizeUpdateEntity(database, previousItem, newItemSet);
                            }
                        }
                    }
                } else if (navigation.CurrentValue == null && newNavigationValue != null) {
                    // Create a new navigation overwritten.
                    // Also update the attached navigators.
                    //AttachDate(newNavigationValue);
                    EntityEntry newNavigationEntry = database.Entry(newNavigationValue);
                    newNavigationEntry.State = EntityState.Added;
                    navigation.CurrentValue = newNavigationValue;
                } else if (navigation.CurrentValue != null && newNavigationValue != null) {
                    // Update the existing navigation overwritten
                    if (navigation.CurrentValue is IEntity currentItemSet && newNavigationValue is IEntity newItemSet) {
                        SanitizeUpdateEntity(database, currentItemSet, newItemSet);
                    }
                }

            }
        }

    }
}
