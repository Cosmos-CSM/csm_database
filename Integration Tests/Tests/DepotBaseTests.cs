using CSM_Database_Core.Depots.Abstractions.Bases;
using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Depots.Models.Structs;

using CSM_Database_Testing.Abstractions.Bases;

using DatabaseProxy;
using DatabaseProxy.Entities;

using Microsoft.EntityFrameworkCore;

namespace Integration_Tests.Tests;

/// <summary>
///     Integration tests class for <see cref="DepotBase{TDatabase, TEntity}"/>
/// </summary>
public class DepotBaseTests
    : DepotIntegrationTestsBase<EntityProxy, DepotProxy, DatabaseProxy.DatabaseProxy> {

    protected override EntityProxy EntityFactory(string Entropy) {
        return new EntityProxy {
        };
    }

    /// <summary>
    ///     Method: <see cref="DepotBase{TDatabase, TEntity}.Update(QueryInput{TEntity, UpdateInput{TEntity}})"/> 
    ///     Expectation: Success updating.
    /// </summary>
    public override async Task Update_Single_Success() {
        //Expectation
        EntityProxy testEntity = await Store(
                new EntityProxy()
            );
        EntityDependencyProxy dependency = await Store(
                new EntityDependencyProxy()
            );

        EntityDependantProxy dependant = await Store(
                new EntityDependantProxy()
            );
        testEntity.EntityDependencyProxy = dependency;

        UpdateOutput<EntityProxy> updateOutput = await _depot.Update(
                new QueryInput<EntityProxy, UpdateInput<EntityProxy>> {
                    Parameters = new UpdateInput<EntityProxy> {
                        Entity = testEntity,
                        Relations = {
                            {
                                nameof(EntityProxy.EntityDependantProxies),
                                [
                                        new RelationUpdate {
                                                Action = RelationUpdateAction.ADD,
                                                Entity = dependant
                                            },
                                    ]
                            }
                        },
                    },
                    PostProcessor = (query) => {
                        return query.Include(i => i.EntityDependantProxies);
                    }
                }
            );

        Assert.NotNull(updateOutput.Original);
        Assert.Empty(updateOutput.Original.EntityDependantProxies);
        Assert.NotEmpty(updateOutput.Updated.EntityDependantProxies);

        Assert.Contains(
                updateOutput.Updated.EntityDependantProxies,
                (dependantEntry) => dependantEntry.Id == dependant.Id
            );
        Assert.Equal(
                dependency.Id,
                updateOutput.Updated.EntityDependencyProxy.Id
            );
    }
}
