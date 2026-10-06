using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json.Serialization;

using CSharp_Database_Extension.Entities.Abstractions.Interfaces;
using CSharp_Database_Extension.Entities.Validation.Abstractions.Bases;

using CSharp_Extension.Common.Abstractions.Bases;

namespace CSharp_Database_Extension;

/// <summary>
///     Represents a business entity.
/// </summary>
public abstract partial class EntityBase
    : ObjectBase, IEntity {

    #region Server Side Properties

    /// <inheritdoc/>
    [NotMapped, JsonPropertyOrder(0)]
    public string Discriminator { get; init; }

    /// <inheritdoc/>
    [NotMapped, JsonIgnore]
    public abstract Type Database { get; init; }

    #endregion


    /// <inheritdoc/>
    public long Id { get; set; }

    /// <inheritdoc/>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <inheritdoc/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string? Description { get; set; }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public EntityBase() {
        Discriminator = $"{GetType().GUID}";
    }

    /// <inheritdoc/>
    protected void Evaluate() {

        foreach (PropertyInfo property in GetType().GetProperties()) {

            IEnumerable<ValidatorBase> attributes = property.GetCustomAttributes<ValidatorBase>();
            if (!attributes.Any())
                return;

            foreach (ValidatorBase validator in attributes) {
                try {
                    validator.Validate(this);
                } catch {

                }
            }
        }
    }

    /// <inheritdoc/>
    public void EvaluateRead() {
        Evaluate();
    }

    /// <inheritdoc/>
    public void EvaluateWrite() {
        Evaluate();
    }

    /// <inheritdoc/>
    public Exception[] EvaluateModel() {
        return [];
    }
}