/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Proxies.Events;

public readonly record struct ValueChangedEventArgs<T>(T OldValue, T NewValue)
{
    public static ValueChangedEventArgs<T> Default =>
        new(default, default);
}
