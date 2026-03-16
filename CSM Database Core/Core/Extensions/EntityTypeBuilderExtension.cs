using System.Reflection;

using CSM_Database_Core.Entities.Abstractions.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSM_Database_Core.Core.Extensions;

/// <summary>
///     Provide extension methods for <see cref="EntityTypeBuilder"/> class type. 
/// </summary>
public static class EntityTypeBuilderExtension {

    /// <summary>
    ///     Creates a relation between two entities into the data storage migration.
    /// </summary>
    /// <param name="entityTypeBuilder">
    ///     Instance of <see cref="DbContext"/> entity type builder, from native Entity Framework configuration.
    /// </param>
    /// <param name="sourceRelationType">
    ///     Type of the source entity for the relation.
    /// </param>
    /// <param name="sourceRef">
    ///     Property reference name from <paramref name="sourceRelationType"/> that will store the <paramref name="targetRelationType"/> object.
    /// </param>
    /// <param name="targetRelationType">
    ///     Type of the target entity for the relation.
    /// </param>
    /// <param name="targetRef">
    ///     Property reference name from <paramref name="targetRelationType"/> that will store the <paramref name="sourceRelationType"/> object. If null, framework will try to calculate the ref
    ///     automatically.
    /// </param>
    /// <param name="isRequired">
    ///     Whether this relation is required at database level.
    /// </param>
    /// <param name="isAutoLoaded">
    ///     Whether this relation will be automatically loaded on each reading event.
    /// </param>
    /// <param name="isIndex">
    ///     Whether this relation must be unique and work as index. This is calculated using both primary keys from each entity.
    /// </param>
    /// <param name="deleteBehavior">
    ///     Behavior when one of the relation is removed.
    /// </param>
    public static void Link(
            this EntityTypeBuilder entityTypeBuilder,
            Type sourceRelationType,
            string sourceRef,
            Type targetRelationType,
            string? targetRef = null,
            bool isRequired = false,
            bool isAutoLoaded = false,
            bool isIndex = false,
            DeleteBehavior deleteBehavior = DeleteBehavior.Restrict
        ) {
        Type entityInterfaceType = typeof(IEntity);

        Type source = sourceRelationType;
        Type target = targetRelationType;

        bool isSourceEntity = source.IsAssignableTo(entityInterfaceType);
        bool isTargetEntity = target.IsAssignableTo(entityInterfaceType);

        if (!(isSourceEntity && isTargetEntity)) {
            throw new Exception($"Relation [Source ({source.Name})] or [Target ({target.Name})] is not a CSM [IEntity]");
        }

        PropertyInfo sourceNavigation = source.GetProperty(sourceRef)
            ?? throw new Exception($"[Source {source.Name}] doesn't contain a navigation reference ({sourceRef})");

        Type sourcePropertyType = sourceNavigation.PropertyType;
        if (sourcePropertyType.IsGenericType && sourcePropertyType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
            throw new Exception($"This method only supports one to one/many relationship for many-to-many use ( #WIP# )");
        }

        string targetReference = targetRef ?? $"{source.Name}";
        PropertyInfo? targetNavigation = target.GetProperty(targetReference);
        if (targetRef == null && targetNavigation == null) {
            targetReference = $"{source.Name}s";
            targetNavigation = target.GetProperty(targetReference);
        }
        string shadowProperty = $"{sourceRef}Shadow";

        Type propType = isRequired
            ? typeof(long)
            : typeof(long?);
        entityTypeBuilder
            .Property(propType, shadowProperty)
            .HasColumnName(sourceRef)
            .HasColumnType("bigint")
            .IsRequired(isRequired);

        ReferenceNavigationBuilder relationBuilder = entityTypeBuilder.HasOne(target, sourceRef);
        Type? targetNavPropType = targetNavigation?.PropertyType;
        if (targetNavPropType != null && targetNavPropType.IsGenericType && targetNavPropType.GetGenericTypeDefinition() == typeof(ICollection<>)) {
            relationBuilder
                .WithMany(targetReference)
                .HasForeignKey(shadowProperty)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);
        } else {
            relationBuilder
                .WithOne(targetNavigation is null ? null : targetReference)
                .HasForeignKey(source, shadowProperty)
                .OnDelete(deleteBehavior)
                .IsRequired(isRequired);
        }

        if (isAutoLoaded) {
            entityTypeBuilder
                .Navigation(sourceRef)
                .AutoInclude();
        }

        if (isIndex && isRequired) {
            entityTypeBuilder
                .HasIndex(shadowProperty)
                .IsUnique();
        }
    }


    /// <summary>
    ///     Creates a relation between two entities into the data storage migration.
    /// </summary>
    /// <typeparam name="TSource">
    ///     Type of the relation source entity.
    /// </typeparam>
    /// <typeparam name="TTarget">
    ///     Type of the relation target entity.
    /// </typeparam>
    /// <param name="entityTypeBuilder">
    ///     Instance of <see cref="DbContext"/> entity type builder, from native Entity Framework configuration.
    /// </param>
    /// <param name="sourceRef">
    ///     Property reference name from <typeparamref name="TSource"/> that will store the <typeparamref name="TTarget"/> object.
    /// </param>
    /// <param name="targetRef">
    ///     Property reference name from <typeparamref name="TTarget"/> that will store the <typeparamref name="TSource"/> object. If null, framework will try to calculate the ref
    ///     automatically.
    /// </param>
    /// <param name="isRequired">
    ///     Whether this relation is required at database level.
    /// </param>
    /// <param name="isAutoLoaded">
    ///     Whether this relation will be automatically loaded on each reading event.
    /// </param>
    /// <param name="isIndex">
    ///     Whether this relation must be unique and work as index. This is calculated using both primary keys from each entity.
    /// </param>
    /// <param name="deleteBehavior">
    ///     Behavior when one of the relation is removed.
    /// </param>
    public static void Link<TSource, TTarget>(this EntityTypeBuilder entityTypeBuilder, string sourceRef, string? targetRef = null, bool isRequired = false, bool isAutoLoaded = false, bool isIndex = false, DeleteBehavior deleteBehavior = DeleteBehavior.Restrict)
        where TSource : class, IEntity
        where TTarget : class, IEntity {

        Link(
                entityTypeBuilder,
                typeof(TSource),
                sourceRef,
                typeof(TTarget),
                targetRef: targetRef,
                isRequired: isRequired,
                isAutoLoaded: isAutoLoaded,
                isIndex: isIndex,
                deleteBehavior: deleteBehavior
            );
    }
}
