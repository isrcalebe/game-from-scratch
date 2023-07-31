/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace miniblocks.API.Extensions.ObjectExtensions;

public static class ObjectExtensions
{
    [return: NotNull]
    public static T AsNonNull<T>(this T? target)
        => target!;

    public static bool IsNull<T>([NotNullWhen(false)] this T target)
        => ReferenceEquals(target, null);

    public static bool IsNotNull<T>([NotNullWhen(true)] this T target)
        => !ReferenceEquals(target, null);
}
