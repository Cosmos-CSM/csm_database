using CSM_Database_Core;
using CSM_Database_Core.Core.Attributes;
using CSM_Database_Core.Core.Extensions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseProxy.Entities;

/// <summary>
///     Reepresents an entity proxy.
/// </summary>
public class EntityProxy
    : EntityBase {

    public override Type Database { get; init; } = typeof(DatabaseProxy);

    [EntityDependency(nameof(EntityDependencyProxy), typeof(EntityDependencyProxy))]
    public EntityDependencyProxy EntityDependencyProxy { get; set; } = default!;


    [EntityDependant(nameof(EntityDependantProxies), typeof(EntityDependantProxy), true)]
    public ICollection<EntityDependantProxy> EntityDependantProxies { get; set; } = [];


    protected override void DesignEntity(EntityTypeBuilder etBuilder) {

        etBuilder.Link<EntityProxy, EntityDependencyProxy>(
                nameof(EntityDependencyProxy),
                targetRef: nameof(EntityDependencyProxy.EntityProxies),
                isAutoLoaded: true
            );
    }
}
