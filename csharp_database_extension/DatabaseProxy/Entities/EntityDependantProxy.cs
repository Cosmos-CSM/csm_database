using CSharp_Database_Extension;
using CSharp_Database_Extension.Common.Attributes;
using CSharp_Database_Extension.Common.Extensions;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DatabaseProxy.Entities;

/// <summary>
///     Represents a dependant entity proxy.
/// </summary>
public class EntityDependantProxy
    : EntityBase {

    public override Type Database { get; init; } = typeof(DatabaseProxy);


    [EntityRelation]
    public EntityProxy EntityProxy { get; init; } = default!;

    protected override void DesignEntity(EntityTypeBuilder etBuilder) {

        etBuilder.Link<EntityDependantProxy, EntityProxy>(
                nameof(EntityProxy),
                targetRef: nameof(EntityProxy.EntityDependantProxies),
                isAutoLoaded: true
            );
    }
}
