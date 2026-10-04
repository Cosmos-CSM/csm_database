using CSM_Foundation_Core.Errors.Abstractions.Bases;

namespace CSM_Database_Core.Core.Errors;

/// <summary>
///     Represents the <see cref="DatabaseError"/> exception events.
/// </summary>
public enum DatabaseErrorEvents {

    /// <summary>
    ///     Event when the activator finds that the configured context has a DbSet with no entity configured.
    /// </summary>
    WRONG_DBSET_ENTITY,

    /// <summary>
    ///     Event when the activator finds that a DbSet has an Entity configured as an interface but it doesn't have correctly an Activator Reference configured.
    /// </summary>
    INTERFACE_UNCONFIGURED,
}


/// <summary>
///     Represents an <see cref="Exception"/> handled by database activation process.
/// </summary>
public class DatabaseError
    : ErrorBase<DatabaseErrorEvents> {

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="event">
    ///     Exception triggering event.
    /// </param>
    /// <param name="data">
    ///     Sensitive analysis data.
    /// </param>
    public DatabaseError(DatabaseErrorEvents @event, IDictionary<string, object?>? data = null)
        : base($"[Database Activation]: {@event}({(int)@event})", @event, data: data) {

    }
}
