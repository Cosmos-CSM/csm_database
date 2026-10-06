using CSharp_Database_Extension.Depots.Abstractions.Interfaces;
using CSharp_Database_Extension.Entities.Abstractions.Interfaces;

namespace CSharp_Database_Extension.Depots.Models;

/// <summary>
///     {model} class for <see cref="ViewOutput{TEntity}"/>.
///     
///     <para>
///         Defines a data model class that represents an {output} object from the <see cref="IDepotView{TEntity}.View(QueryInput{TEntity, ViewInput{TEntity}})"/> operation along different
///         <see cref="IEntity"/> implementations.
///     </para>
/// </summary>
/// <typeparam name="TEntity">
///     type of the <see cref="IEntity"/> implementation the <see cref="IDepotView{TEntity}.View(QueryInput{TEntity, ViewInput{TEntity}})"/> was called for.
/// </typeparam>
public class ViewOutput<TEntity>
    where TEntity : IEntity {
    /// <summary>
    ///     The collection of items gathered.
    /// </summary>
    public required TEntity[] Entities {
        get;
        init {
            field = value;
            Length = value.Length;
        }
    } = [];

    /// <summary>
    ///     The available pages.
    /// </summary>
    public required int Pages { get; init; }

    /// <summary>
    ///     The current page.
    /// </summary>
    public required int Page { get; init; }

    /// <summary>
    ///     Indicates the timemark when was created.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    ///     Indicates the quantity of records that this result contains.
    /// </summary>
    public int Length { get; init; }

    /// <summary>
    ///     Count of total records that currently exist at the live database
    /// </summary>
    public required int Count { get; init; }
}
