using System.Reflection;

using CSM_Foundation.Database;

using CSM_Foundation_Database.Extensions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSM_Foundation_Database.Entities.Bases;

/// <summary>
///      Represents an <see cref="IEntity"/> scope for a <see cref="IPartnerBridgeEntity"/>.
/// </summary>
public interface IPartnerScopeEntity
    : IEntity {
}

/// <summary>
///      Represents an <see cref="IEntity"/> scope for a <see cref="IPartnerBridgeEntity"/>.
/// </summary>
/// <typeparam name="TPartnerBridgeEntity">
///     Type of the <see cref="IPartnerBridgeEntity"/>.
/// </typeparam>
public interface IPartnerScopeEntity<TPartnerBridgeEntity>
    : IPartnerScopeEntity
    where TPartnerBridgeEntity : IPartnerBridgeEntity {

    /// <summary>
    ///     <typeparamref name="TPartnerBridgeEntity"/> data.
    /// </summary>
    public TPartnerBridgeEntity Bridge { get; set; }
}

/// <summary>
///      Represents an <see cref="IEntity"/> scope for a <see cref="IPartnerBridgeEntity"/>.
/// </summary>
/// <typeparam name="TPartnerBridgeEntity">
///     Type of the <see cref="IPartnerBridgeEntity"/>.
/// </typeparam>
public abstract class BPartnerScopeEntity<TPartnerBridgeEntity>
    : BEntity, IPartnerScopeEntity<TPartnerBridgeEntity>
    where TPartnerBridgeEntity : IPartnerBridgeEntity {

    public TPartnerBridgeEntity Bridge { get; set; } = default!;


    protected virtual void DesignScopeEntity(EntityTypeBuilder etBuilder) { }

    protected internal override void DesignEntity(EntityTypeBuilder etBuilder) {
        PropertyInfo[] commonEntityProperties = typeof(TPartnerBridgeEntity).GetProperties();

        foreach (PropertyInfo commonEntityProperty in commonEntityProperties) {

            if (commonEntityProperty.PropertyType != GetType()) {
                continue;
            }

            etBuilder.Link(
                Relation: (GetType(), typeof(TPartnerBridgeEntity)),
                SourceReference: nameof(Bridge),
                TargetReference: commonEntityProperty.Name,
                Required: true,
                Index: true,
                Auto: true
            );
        }

        DesignScopeEntity(etBuilder);
    }
}
