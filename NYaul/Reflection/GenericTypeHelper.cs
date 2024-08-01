using System.Collections.Generic;
using System;
using System.Linq;

public record GenericArgumentResult(Type RequestedType, Type DirectSubclass, Type[] GenericArguments);

public static class GenericTypeHelper
{
    /// <summary>
    /// Returns <see cref="TEntity"/> of <see cref="EntityTypeBuilder{TEntity}"/>,
    /// even from <see cref="EntityTypeBuilderOfEntity"/> : <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="concreteType">For e.g EntityTypeBuilderOfEntity.</param>
    /// <param name="genericDefinition">for e.g typeof(<see cref="EntityTypeBuilder{}"/>).</param>
    /// <returns>An enumerable of generic arguments of <paramref name="genericDefinition"/>.</returns>
    public static IEnumerable<GenericArgumentResult> GetGenericArgumentsOfTypeDefinition(
        Type concreteType,
        Type genericDefinition
    )
    {
        return EnumerateBaseTypesAndInterfaces(concreteType)
            .Where(x => x.IsGenericType)
            .Where(x => x.GetGenericTypeDefinition() == genericDefinition)
            .Select(x => new GenericArgumentResult(
                RequestedType: concreteType,
                DirectSubclass: x,
                GenericArguments: x.GenericTypeArguments
            ));
    }

    /// <summary>
    /// Returns <see cref="TEntity"/> of <see cref="EntityTypeBuilder{TEntity}"/>,
    /// even from <see cref="EntityTypeBuilderOfEntity"/> : <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="concreteType">For e.g EntityTypeBuilderOfEntity.</param>
    /// <param name="genericDefinition">for e.g typeof(<see cref="EntityTypeBuilder{}"/>).</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <returns>An enumerable of generic arguments of <paramref name="genericDefinition"/>.</returns>
    public static GenericArgumentResult? GetFirstGenericArgumentsOfTypeDefinition(
        Type concreteType,
        Type genericDefinition,
        Func<GenericArgumentResult, bool>? predicate = null
    )
    {
        var result = GetGenericArgumentsOfTypeDefinition(concreteType, genericDefinition);
        if (predicate != null)
        {
            result = result.Where(predicate);
        }

        return result.FirstOrDefault();
    }

    private static IEnumerable<Type> EnumerateBaseTypesAndInterfaces(Type? type, bool returnInput = true)
    {
        if (type == null)
        {
            yield break;
        }

        if (returnInput)
        {
            yield return type;
        }

        // Return all base types
        var current = type.BaseType;
        while (current != null)
        {
            foreach (var interfaceType in current.GetInterfaces())
            {
                yield return interfaceType;
            }

            yield return current;
            current = current.BaseType;
        }

        // Return all interfaces
        foreach (var interfaceType in type.GetInterfaces())
        {
            yield return interfaceType;
        }
    }
}