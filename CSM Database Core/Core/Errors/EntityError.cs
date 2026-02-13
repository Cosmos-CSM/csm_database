using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Errors.Abstractions.Bases;

namespace CSM_Database_Core.Core.Errors;

/// <summary>
///     Represents <see cref="EntityError{TEntity}"/> exception events.
/// </summary>
public enum EntityErrorEvents {
    /// <summary>
    ///     Event when the Entity read operation has failed.
    /// </summary>
    READ_FAILED,

    /// <summary>
    ///     Event when the Entity evaluation during a read operation has failed.
    /// </summary>
    READ_VALIDATION_FAILED,

    /// <summary>
    ///     Event when the Entity create operation has failed.
    /// </summary>
    CREATE_FAILED,

    /// <summary>
    ///     Event when the Entity delete operation has failed.
    /// </summary>
    DELETE_FAILED,
}

/// <summary>
///     Exception that represents an error occurred over an <see cref="IEntity"/> operation.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the <see cref="IEntity"/> that failed.
/// </typeparam>
public class EntityError<TEntity>
    : ErrorBase<EntityErrorEvents>
    where TEntity : IEntity {

    /// <summary>
    ///     Entity instance that has caused the error.
    /// </summary>
    public TEntity Entity { get; init; } = default!;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="entity">
    ///     Entity instance that has caused the error.
    /// </param>
    /// <param name="exception">
    ///     <see cref="Exception"/> caught over the operation error.
    /// </param>
    public EntityError(EntityErrorEvents @event, TEntity entity, Exception exception)
        : base($"An entity ({entity.GetType()}) operation has failed", @event, exception) {

        Entity = entity;

        Data.Add("Entity", entity);
    }
}