using System;
using System.Collections.Generic;
using System.Reflection;

namespace NYaul.Extensions;

public static class ReflectionExtensions
{
    /// <inheritdoc cref="GenericTypeHelper.GetGenericArgumentsOfTypeDefinition(Type, Type)"/>
    public static IEnumerable<GenericArgumentResult> GetGenericArgumentsOfTypeDefinition(
        this Type concreteType,
        Type genericDefinition
    )
    {
        return GenericTypeHelper.GetGenericArgumentsOfTypeDefinition(concreteType, genericDefinition);
    }

    /// <inheritdoc cref="GenericTypeHelper.GetFirstGenericArgumentsOfTypeDefinition(Type, Type, Func{GenericArgumentResult, bool}?)"/>
    public static GenericArgumentResult? GetFirstGenericArgumentsOfTypeDefinition(
        this Type concreteType,
        Type genericDefinition,
        Func<GenericArgumentResult, bool>? predicate = null
    )
    {
        return GenericTypeHelper.GetFirstGenericArgumentsOfTypeDefinition(concreteType, genericDefinition, predicate);
    }

    /// <summary>
    /// Changes the generic type arguments of a method
    /// (For eg: <see cref="Func{string}"/> to <see cref="Func{Tint}"/>).
    /// </summary>
    /// <param name="methodInfo">The method information.</param>
    /// <returns>the new MethodInfo.</returns>
    /// <param name="types">The types.</param>
    public static MethodInfo RewriteGenericType(this MethodInfo methodInfo, params Type[] types)
    {
        if (!methodInfo.IsGenericMethod)
        {
            throw new ArgumentException("The method is not generic.", nameof(methodInfo));
        }

        var template = methodInfo;
        if (!methodInfo.IsGenericMethodDefinition)
        {
            template = methodInfo.GetGenericMethodDefinition();
        }

        return template.MakeGenericMethod(types);
    }
}