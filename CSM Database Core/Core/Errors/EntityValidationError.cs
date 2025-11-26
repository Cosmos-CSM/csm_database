using CSM_Database_Core.Depots.Models;

using CSM_Foundation_Core.Errors.Abstractions.Bases;

namespace CSM_Database_Core.Core.Errors;

/// <summary>
///     Represents <see cref="EntityValidationError"/> events.
/// </summary>
public enum EntityValidationErrorEvents {

    /// <summary>
    ///     Event when entity read validation has failed.
    /// </summary>
    READ_FAILED,

    /// <summary>
    ///     Event when entity write validation has failed.
    /// </summary>
    WRITE_FAILED,
}

/// <summary>
///     Represents and error occurred during entity validation.
/// </summary>
public class EntityValidationError
    : ErrorBase<EntityValidationErrorEvents> {

    /// <summary>
    ///     Runtime type of the Entity that failed.
    /// </summary>
    public Type EntityType;

    /// <summary>
    ///     Validations results. 
    /// </summary>
    public PropertyValidationResult[] Results;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="entityType">
    public EntityValidationError(Type entityType, EntityValidationErrorEvents @event, PropertyValidationResult[] results)
        : base($"Entity ({entityType}) validation has failed.", @event) {

        Results = results;
        EntityType = entityType;

        Data.Add(nameof(Results), results);
        Data.Add(nameof(EntityType), entityType);
    }
}
