using CSharp_Database_Extension.Depots.Abstractions.Interfaces;
using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Depots.Models;

/// <summary>
///     [Record] for specific <see cref="IDepot{TEntity}"/> operations,
///     is a required parameters for operations related with database data management.
/// </summary>
public record QueryInput<TEntity, TParameters>
    where TEntity : IEntity {

    /// <summary>
    ///    Custom operation scope input parameters information.
    /// </summary>
    public required TParameters Parameters { get; init; }

    /// <summary>
    ///     Custom query process to apply before the operation commit.
    /// </summary>
    public QueryProcessor<TEntity>? PreProcessor { get; set; }

    /// <summary>
    ///     Custom query process to apply after the operation commit.
    /// </summary>
    public QueryProcessor<TEntity>? PostProcessor { get; set; }
}
