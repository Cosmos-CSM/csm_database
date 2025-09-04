using CSM_Foundation.Database;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSM_Foundation_Database.Entities.Bases;

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
///     Type of the <see cref="IPartnerScopeEntity"/> that represents the internal data model.
/// </typeparam>
/// <typeparam name="TExternalScope">
///     Type of the <see cref="IPartnerScopeEntity"/> that represents the external data model.
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

/// <summary>
///     Represents an <see cref="IEntity"/> that holds a scope between internal ( tenant business entities data ) and external ( tenant partners entities data ).
/// </summary>
/// <typeparam name="TInternalScope"> 
///     Type of the <see cref="IEntity"/> that represents the internal data model.
/// </typeparam>
/// <typeparam name="TExternalScope">
///     Type of the <see cref="IEntity"/> that represents the external data model.
/// </typeparam>
public abstract class BPartnerBridgeEntity<TInternalScope, TExternalScope>
    : BEntity, IPartnerBridgeEntity<TInternalScope, TExternalScope>
    where TInternalScope : IPartnerScopeEntity
    where TExternalScope : IPartnerScopeEntity {

    public TInternalScope? Internal { get; set; }

    public TExternalScope? External { get; set; }

    protected internal override void DesignEntity(EntityTypeBuilder etBuilder) {
        string referenceName = nameof(IPartnerScopeEntity<IPartnerBridgeEntity>.Bridge);

        etBuilder
            .HasOne(typeof(TExternalScope), nameof(External))
            .WithOne(referenceName)
            .HasForeignKey(typeof(TExternalScope), $"{referenceName}Shadow");


        etBuilder
            .HasOne(typeof(TInternalScope), nameof(Internal))
            .WithOne(referenceName)
            .HasForeignKey(typeof(TInternalScope), $"{referenceName}Shadow");


        etBuilder
            .Navigation(nameof(External))
            .AutoInclude();

        etBuilder
            .Navigation(nameof(Internal))
            .AutoInclude();

        DesignCommonEntity(etBuilder);
    }

    protected abstract void DesignCommonEntity(EntityTypeBuilder etBuilder);

}
