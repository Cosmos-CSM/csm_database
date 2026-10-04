using CSM_Database_Core.Validation.Abstractions.Interfaces;

namespace CSM_Database_Core.Validation.Abstractions.Bases;

/// <inheritdoc cref="IValidator"/>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ValidatorBase
    : Attribute, IValidator {

    /// <inheritdoc/>
    public abstract bool ValidateType(Type Type);

    /// <inheritdoc/>
    public abstract bool Validate(object? value);
}
