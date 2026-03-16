using System.Reflection;

using CSM_Database_Core.Core.Extensions;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSM_Database_Core.Entities.Abstractions.Bases;

/// <summary>
///      Represents an <see cref="IEntity"/> scope for a <see cref="IPartnerBridgeEntity"/>.
/// </summary>
/// <typeparam name="TPartnerBridgeEntity">
///     Type of the <see cref="IPartnerBridgeEntity"/>.
/// </typeparam>
public abstract class PartnerScopeEntityBase<TPartnerBridgeEntity>
    : EntityBase, IPartnerScopeEntity<TPartnerBridgeEntity>
    where TPartnerBridgeEntity : IPartnerBridgeEntity {


    /// <inheritdoc/>
    public TPartnerBridgeEntity Bridge { get; set; } = default!;

    /// <inheritdoc/>
    protected virtual void DesignScopeEntity(EntityTypeBuilder etBuilder) { }

    /// <inheritdoc/>
    protected internal override void DesignEntity(EntityTypeBuilder etBuilder) {
        PropertyInfo[] commonEntityProperties = typeof(TPartnerBridgeEntity).GetProperties();

        foreach (PropertyInfo commonEntityProperty in commonEntityProperties) {

            if (commonEntityProperty.PropertyType != GetType()) {
                continue;
            }

            etBuilder.Link(
                sourceRelationType: GetType(),
                targetRelationType: typeof(TPartnerBridgeEntity),
                sourceRef: nameof(Bridge),
                targetRef: commonEntityProperty.Name,
                isRequired: true,
                isIndex: true,
                isAutoLoaded: true
            );
        }

        DesignScopeEntity(etBuilder);
    }
}
