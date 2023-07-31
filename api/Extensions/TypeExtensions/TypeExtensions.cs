/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace miniblocks.API.Extensions.TypeExtensions;

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, Type?> underlying_type_cache = new();

    /// <summary>
    /// Returns a human-readable name for the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to get the name for.</param>
    /// <returns>A human-readable name for the specified <see cref="Type"/>.</returns>
    public static string ReadableName(this Type type)
        => getReadableName(type, new HashSet<Type>());

    /// <summary>
    /// Enumerates all base types of the specified <see cref="Type"/>, starting from the immediate base type and ending at <see cref="object"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to enumerate the base types for.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Type"/> objects representing the base types of the specified <see cref="Type"/>.</returns>
    public static IEnumerable<Type?> EnumerateBaseTypes(this Type? type)
    {
        while (type != null && type != typeof(object))
        {
            yield return type;

            type = type.BaseType;
        }
    }

    /// <summary>
    /// Returns the access modifier of the specified <see cref="FieldInfo"/>.
    /// </summary>
    /// <param name="info">The <see cref="FieldInfo"/> to get the access modifier for.</param>
    /// <returns>An <see cref="AccessModifier"/> value representing the access modifier of the specified <see cref="FieldInfo"/>.</returns>
    public static AccessModifier GetAccessModifier(this FieldInfo info)
    {
        var result = AccessModifier.None;

        if (info.IsPublic)
            result |= AccessModifier.Public;
        if (info.IsAssembly)
            result |= AccessModifier.Internal;
        if (info.IsFamily)
            result |= AccessModifier.Protected;
        if (info.IsPrivate)
            result |= AccessModifier.Private;
        if (info.IsFamilyOrAssembly)
            result |= AccessModifier.Protected | AccessModifier.Internal;

        return result;
    }

    /// <summary>
    /// Returns the access modifier of the specified <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="info">The <see cref="MethodInfo"/> to get the access modifier for.</param>
    /// <returns>An <see cref="AccessModifier"/> value representing the access modifier of the specified <see cref="MethodInfo"/>.</returns>
    public static AccessModifier GetAccessModifier(this MethodInfo info)
    {
        var result = AccessModifier.None;

        if (info.IsPublic)
            result |= AccessModifier.Public;
        if (info.IsAssembly)
            result |= AccessModifier.Internal;
        if (info.IsFamily)
            result |= AccessModifier.Protected;
        if (info.IsPrivate)
            result |= AccessModifier.Private;
        if (info.IsFamilyOrAssembly)
            result |= AccessModifier.Protected | AccessModifier.Internal;

        return result;
    }

    /// <summary>
    /// Returns the underlying type argument of the specified nullable <see cref="Type"/>, or <c>null</c> if the specified <see cref="Type"/> is not nullable.
    /// </summary>
    /// <param name="type">The nullable <see cref="Type"/> to get the underlying type argument for.</param>
    /// <returns>The underlying type argument of the specified nullable <see cref="Type"/>, or <c>null</c> if the specified <see cref="Type"/> is not nullable.</returns>
    public static Type? GetUnderlyingNullableType(this Type type)
        => !type.IsGenericType ? null : underlying_type_cache.GetOrAdd(type, nullable => Nullable.GetUnderlyingType(type));

    /// <summary>
    /// Determines whether the specified <see cref="Type"/> is nullable.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check.</param>
    /// <returns><c>true</c> if the specified <see cref="Type"/> is nullable; otherwise, <c>false</c>.</returns>
    public static bool IsNullable(this Type type)
        => type.GetUnderlyingNullableType() != null;

    public static bool IsNullable(this EventInfo info)
    {
        if (info.EventHandlerType != null && IsNullable(info.EventHandlerType))
            return true;

        return isNullableInfo(new NullabilityInfoContext().Create(info));
    }

    /// <summary>
    /// Determines whether the specified <see cref="ParameterInfo"/> is nullable.
    /// </summary>
    /// <param name="info">The <see cref="ParameterInfo"/> to check.</param>
    /// <returns><c>true</c> if the specified <see cref="ParameterInfo"/> is nullable; otherwise, <c>false</c>.</returns>
    public static bool IsNullable(this ParameterInfo info)
        => IsNullable(info.ParameterType) || isNullableInfo(new NullabilityInfoContext().Create(info));

    /// <summary>
    /// Determines whether the specified <see cref="FieldInfo"/> is nullable.
    /// </summary>
    /// <param name="info">The <see cref="FieldInfo"/> to check.</param>
    /// <returns><c>true</c> if the specified <see cref="FieldInfo"/> is nullable; otherwise, <c>false</c>.</returns>
    public static bool IsNullable(this FieldInfo info)
        => IsNullable(info.FieldType) || isNullableInfo(new NullabilityInfoContext().Create(info));

    /// <summary>
    /// Determines whether the specified <see cref="PropertyInfo"/> is nullable.
    /// </summary>
    /// <param name="info">The <see cref="PropertyInfo"/> to check.</param>
    /// <returns><c>true</c> if the specified <see cref="PropertyInfo"/> is nullable; otherwise, <c>false</c>.</returns>
    public static bool IsNullable(this PropertyInfo info)
        => IsNullable(info.PropertyType) || isNullableInfo(new NullabilityInfoContext().Create(info));

    private static string getReadableName(Type type, ISet<Type> usedTypes)
    {
        usedTypes.Add(type);

        var result = type.Name;
        var cursor = type.Name.IndexOf('`');
        if (cursor > 0)
            result = result[..cursor];

        if (type.DeclaringType != null && !usedTypes.Contains(type.DeclaringType))
            result = $"{getReadableName(type.DeclaringType, usedTypes)}+{result}";

        if (!type.IsGenericType) return result;

        if (type.GetGenericArguments().Except(usedTypes) is Type[] arguments && arguments.Any())
            result += $"<{string.Join(',', arguments.Select(generic => getReadableName(generic, usedTypes)))}>";

        return result;
    }

    private static bool isNullableInfo(NullabilityInfo info)
        => info.WriteState == NullabilityState.Nullable
           || info.ReadState == NullabilityState.Nullable;
}
