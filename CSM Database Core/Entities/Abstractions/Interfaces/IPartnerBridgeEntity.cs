using CSM_Database_Core.Entities.Abstractions.Bases;

namespace CSM_Database_Core.Entities.Abstractions.Interfaces;

/// <summary>
///     Represents an <see cref="IEntity"/> as a bridge between a possible internal business entity or a partner provided entity data.
/// </summary>
public interface IPartnerBridgeEntity
    : IEntity {
}

/// <summary>
///     Represents an <see cref="IEntity"/> as a bridge between a possible internal business entity or a partner provided entity data.
/// </summary>
/// <typeparam name="TInternalScope"> 
///     Type of the <see cref="Bases.IPartnerScopeEntity"/> that represents the internal data model.
/// </typeparam>
/// <typeparam name="TExternalScope">
///     Type of the <see cref="Bases.IPartnerScopeEntity"/> that represents the external data model.
/// </typeparam>
public interface IPartnerBridgeEntity<TInternalScope, TExternalScope>
    : IPartnerBridgeEntity
    where TInternalScope : IPartnerScopeEntity
    where TExternalScope : IPartnerScopeEntity {

    /// <summary>
    ///     <typeparamref name="TInternalScope"/> data.
    /// </summary>
    public TInternalScope? Internal { get; set; }

    /// <summary>
    ///     <typeparamref name="TExternalScope"/> data.
    /// </summary>
    public TExternalScope? External { get; set; }
}