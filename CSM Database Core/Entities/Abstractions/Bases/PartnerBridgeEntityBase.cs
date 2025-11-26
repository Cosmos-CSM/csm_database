using CSM_Database_Core.Entities.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///     Represents an <see cref="IEntity"/> that holds a scope between internal ( tenant business entities data ) and external ( tenant partners entities data ).
/// </summary>
/// <typeparam name="TInternalScope"> 
///     Type of the <see cref="IEntity"/> that represents the internal data model.
/// </typeparam>
/// <typeparam name="TExternalScope">
///     Type of the <see cref="IEntity"/> that represents the external data model.
/// </typeparam>
public abstract class PartnerBridgeEntityBase<TInternalScope, TExternalScope>
    : EntityBase, IPartnerBridgeEntity<TInternalScope, TExternalScope>
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
