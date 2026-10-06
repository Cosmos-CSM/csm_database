using CSharp_Database_Extension;
using CSharp_Database_Extension.Depots.Abstractions.Bases;

using CSharp_Extension.Common.Abstractions.Interfaces;

using DatabaseProxy.Entities;

namespace DatabaseProxy;

public class DepotProxy
    : DepotBase<DatabaseProxy, EntityProxy> {

    public DepotProxy(DatabaseProxy Database, IDisposer<IEntity>? Disposer) : base(Database, Disposer) {
    }
}
