using CSM_Database_Core.Depots.Abstractions.Bases;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using CSM_Foundation_Core.Abstractions.Interfaces;

using DatabaseProxy.Entities;

namespace DatabaseProxy;

public class DepotProxy
    : DepotBase<DatabaseProxy, EntityProxy> {

    public DepotProxy(DatabaseProxy Database, IDisposer<IEntity>? Disposer) : base(Database, Disposer) {
    }
}
