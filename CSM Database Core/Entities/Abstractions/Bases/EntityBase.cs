using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.Json.Serialization;

using CSM_Database_Core.Entities.Abstractions.Interfaces;
using CSM_Database_Core.Validation.Abstractions.Bases;

using CSM_Foundation_Core.Abstractions.Bases;

namespace CSM_Database_Core;

/// <summary>
///     Represents a tenant business live stored entity model, that usually are objects wich data are grouped by bound that 
///     instrinsictly defines their own.
///     
///     <para>
///         This abtract base provides { CSM } built-in behaviors for a very low level <see cref="IEntity"/> implementation
///     </para>
/// </summary>
public abstract partial class EntityBase
    : ObjectBase<IEntity>, IEntity {

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
    public Exception[] EvaluateDefinition() {
        return [];
    }
}

/// <inheritdoc cref="EntityBase"/>
public abstract class EntityBase<TEntity>
    : ObjectBase<TEntity>,
    IEntity
    where TEntity : IEntity {

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
    public Exception[] EvaluateDefinition() {
        return [];
    }
}