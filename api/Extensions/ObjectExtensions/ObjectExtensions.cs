/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace miniblocks.API.Extensions.ObjectExtensions;

/// <summary>
/// A set of extension methods for <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Returns the non-null value of the target object, or throws a <see cref="NullReferenceException"/> if the target is null.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="target">The target object.</param>
    /// <returns>The non-null value of the target object.</returns>
    /// <exception cref="NullReferenceException">Thrown when the target object is null.</exception>
    [return: NotNull]
    public static T AsNonNull<T>(this T? target)
        => target! ?? throw new NullReferenceException($"Expected \"{nameof(target)}\" to be non-null.");

    /// <summary>
    /// Determines whether the target object is null.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="target">The target object.</param>
    /// <returns>True if the target object is null, false otherwise.</returns>
    public static bool IsNull<T>([NotNullWhen(false)] this T target)
        => ReferenceEquals(target, null);

    /// <summary>
    /// Determines whether the target object is not null.
    /// </summary>
    /// <typeparam name="T">The type of the target object.</typeparam>
    /// <param name="target">The target object.</param>
    /// <returns>True if the target object is not null, false otherwise.</returns>
    public static bool IsNotNull<T>([NotNullWhen(true)] this T target)
        => !ReferenceEquals(target, null);
}
