using System.Reflection;

using CSharp_Database_Extension.Common.Errors;

namespace CSharp_Database_Extension.Depots.Models;

/// <summary>
///     Represents an Entity property validation result.
/// </summary>
public record PropertyValidationResult {

    /// <summary>
    ///     Property data.
    /// </summary>
    required public PropertyInfo Property { get; init; }

    /// <summary>
    ///     Property collected errors.
    /// </summary>
    required public ValidatorError[] Errors { get; set; }
}
