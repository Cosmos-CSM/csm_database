using CSharp_Database_Extension;
using CSharp_Database_Extension.Common.Attributes;

namespace DatabaseProxy.Entities;


/// <summary>
///     Represents a dependency entity proxy.
/// </summary>
public class EntityDependencyProxy
    : EntityBase {

    public override Type Database { get; init; } = typeof(DatabaseProxy);

    [EntityRelation]
    public ICollection<EntityProxy> EntityProxies { get; set; } = [];
}
