using System.Reflection;

using CSM_Database_Core.Entities.Abstractions.Interfaces;
using CSM_Database_Core.Validation.Abstractions.Bases;

using CSM_Foundation_Core.Errors.Abstractions.Bases;

namespace CSM_Database_Core.Core.Errors;

/// <summary>
///     Represents <see cref="ValidatorError"/> events.
/// </summary>
public enum ValidatorErrorEvents {
    INVALID,
}

/// <summary>
///     Represents an error ocurred during an Entity property validation. 
/// </summary>
public class ValidatorError
    : ErrorBase<ValidatorErrorEvents> {

    /// <summary>
    ///     Validator instance that threw the error.
    /// </summary>
    public ValidatorBase Validator { get; init; }

    /// <summary>
    ///     Entity property that has failed validation.
    /// </summary>
    public PropertyInfo Property { get; init; }

    /// <summary>
    ///     Property entity instance.
    /// </summary>
    public IEntity Entity { get; init; }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="validator">
    ///     Validator instance that has thrown the error.
    /// </param>
    /// <param name="property">
    ///     Entity property that has failed validation.
    /// </param>
    /// <param name="message">
    ///     Validation message.
    /// </param>
    public ValidatorError(ValidatorBase validator, PropertyInfo property, IEntity entity, string message)
        : base(message, ValidatorErrorEvents.INVALID) {

        Entity = entity;
        Property = property;
        Validator = validator;

        Data.Add(nameof(Entity), entity);
        Data.Add(nameof(Property), property);
        Data.Add(nameof(Validator), validator);
    }
}
