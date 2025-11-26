namespace CSM_Database_Core.Validation.Abstractions.Interfaces;

/// <summary>
///     Represents a property validator instruction.
/// </summary>
public interface IValidator {

    /// <summary>
    ///     Validates whether the given <paramref name="type"/> is acceptable for this validator.
    /// </summary>
    /// <param name="type"> 
    ///     Type to validate. 
    /// </param>
    /// <returns>
    ///     Whether the given <paramref name="type"/> is acceptable for this validator.
    /// </returns>
    public bool ValidateType(Type type);

    /// <summary>
    ///     Validates whether the given <paramref name="value"/> meets the validation criteria.
    /// </summary>
    /// <param name="value">
    ///     Value to validate.
    /// </param>
    /// <returns>
    ///     Whether the given <paramref name="value"/> is valid.
    /// </returns>
    public bool Validate(object? value);
}
