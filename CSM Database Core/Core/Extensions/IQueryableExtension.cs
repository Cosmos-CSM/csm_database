using System.Linq.Expressions;

using System.Reflection;

using CSM_Database_Core.Depots.Abstractions.Interfaces;
using CSM_Database_Core.Depots.Models;
using CSM_Database_Core.Entities.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace CSM_Database_Core.Core.Extensions;

/// <summary>
///     Extension class for <see cref="IQueryable{T}"/> objects.
/// </summary>
public static class IQueryableExtension {

    /// <summary>
    ///     Filters the current query object using the provided view <paramref name="filters"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity being queried.
    /// </typeparam>
    /// <param name="query">
    ///     Current query object.
    /// </param>
    /// <param name="filters">
    ///     View filters instructions.
    /// </param>
    /// <returns>
    ///     A view filtered query object.
    /// </returns>
    public static IQueryable<TEntity> FilterView<TEntity>(this IQueryable<TEntity> query, IViewFilterNode<TEntity>[] filters)
        where TEntity : IEntity {

        if (filters.Length > 0) {
            var orderedFilters = filters.OrderBy(
                    (filter) => filter.Order
                );

            foreach (IViewFilterNode<TEntity> filter in orderedFilters) {
                Expression<Func<TEntity, bool>> queryExpression = filter.Compose();
                query = query.Where(queryExpression);
            }
        }

        return query;
    }

    /// <summary>
    ///     Orders the current query object using the provided view <paramref name="orderings"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity being queried.
    /// </typeparam>
    /// <param name="query">
    ///     Current query object.
    /// </param>
    /// <param name="orderings">
    ///     View orderings instructions.
    /// </param>
    /// <returns>
    ///     A view ordered query object.
    /// </returns>
    /// <exception cref="TypeAccessException"></exception>
    public static IQueryable<TEntity> OrderView<TEntity>(this IQueryable<TEntity> query, ViewOrdering[] orderings)
        where TEntity : IEntity {
        int orderingsCount = orderings.Length;
        if (orderingsCount <= 0) {
            return query;
        }

        Type entityDeclarationType = typeof(TEntity);
        IOrderedQueryable<TEntity> orderingQuery = default!;
        for (int orderingsIteration = 0; orderingsIteration < orderingsCount; orderingsIteration++) {
            ParameterExpression parameterExpression = Expression.Parameter(entityDeclarationType, $"X{orderingsIteration}");
            ViewOrdering ordering = orderings[orderingsIteration];

            PropertyInfo property = entityDeclarationType.GetProperty(ordering.Property)
                ?? throw new TypeAccessException($"Unexist property ({ordering.Property}) on ({entityDeclarationType})");

            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, property);
            UnaryExpression translationExpression = Expression.Convert(memberExpression, typeof(object));
            Expression<Func<TEntity, object>> orderingExpression = Expression.Lambda<Func<TEntity, object>>(translationExpression, parameterExpression);
            if (orderingsIteration == 0) {
                orderingQuery = ordering.Ordering switch {
                    ViewOrderings.Ascending => query.OrderBy(orderingExpression),
                    ViewOrderings.Descending => query.OrderByDescending(orderingExpression),
                    _ => query.OrderBy(orderingExpression),
                };
                continue;
            }

            orderingQuery = ordering.Ordering switch {
                ViewOrderings.Ascending => orderingQuery.ThenBy(orderingExpression),
                ViewOrderings.Descending => orderingQuery.ThenByDescending(orderingExpression),
                _ => orderingQuery.ThenBy(orderingExpression),
            };
        }
        return orderingQuery;
    }

    /// <summary>
    ///     Paginates the current query object using the provided view <paramref name="page"/> and <paramref name="range"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity being queried.
    /// </typeparam>
    /// <param name="query">
    ///     Current query object.
    /// </param>
    /// <param name="page">
    ///     Page number to get.
    /// </param>
    /// <param name="range">
    ///     Range of items per page.
    /// </param>
    /// <param name="export">
    ///     Whether the view pagination is for an exported file.
    /// </param>
    /// <returns>
    ///     A view paginated query object.
    /// </returns>
    public static async Task<PaginationOutput<TEntity>> PaginateView<TEntity>(this IQueryable<TEntity> query, int page, int range, bool export = false)
        where TEntity : class, IEntity {
        int entitiesCount = await query.CountAsync();
        if (export) {

            return new PaginationOutput<TEntity> {
                Query = query,
                PagesCount = 1,
                EntitiesCount = entitiesCount,
            };
        }

        (int pages, int remainder) = Math.DivRem(entitiesCount, range);
        if (remainder > 0) {
            pages++;
        }

        int paginationStart = range * (page - 1);
        int paginationEnd = page == pages ? remainder == 0 ? range : remainder : range;
        query = query
            .AsNoTracking()
            .Skip(paginationStart)
            .Take(paginationEnd);

        return new PaginationOutput<TEntity> {
            Query = query,
            PagesCount = pages,
            EntitiesCount = entitiesCount,
        };
    }


    /// <summary>
    ///     Processes the given <paramref name="query"/> using the provided <paramref name="input"/> and <paramref name="process"/> function.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity being queried.
    /// </typeparam>
    /// <typeparam name="TParameters">
    ///     Process parameters type.
    /// </typeparam>
    /// <param name="query">
    ///     Current query object.
    /// </param>
    /// <param name="input">
    ///     Query processor instructions.
    /// </param>
    /// <param name="process">
    ///     Bootstrapped process function.
    /// </param>
    /// <returns>
    ///     A fully bootstrapped processed query object.
    /// </returns>
    public static IQueryable<TEntity> Process<TEntity, TParameters>(this IQueryable<TEntity> query, QueryInput<TEntity, TParameters> input, Func<IQueryable<TEntity>, IQueryable<TEntity>> process)
        where TEntity : IEntity {

        if (input.PreProcessor != null) {
            query = input.PreProcessor(query);
        }

        query = process(query);

        if (input.PostProcessor != null) {
            query = input.PostProcessor(query);
        }

        return query;
    }
}
