using System.Reflection;

using CSM_Database_Core.Core.Errors;

namespace CSM_Database_Core.Depots.Models;

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
