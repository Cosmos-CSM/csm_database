using CSharp_Database_Extension.Validation.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Validation.Abstractions.Bases;

/// <inheritdoc cref="IValidator"/>
[AttributeUsage(AttributeTargets.Property)]
public abstract class ValidatorBase
    : Attribute, IValidator {

    /// <inheritdoc/>
    public abstract bool ValidateType(Type Type);

    /// <inheritdoc/>
    public abstract bool Validate(object? value);
}
