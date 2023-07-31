/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Proxies.Events;

public readonly struct ValueChangedEvent<T>
{
    public T OldValue { get; }

    public T NewValue { get; }

    public ValueChangedEvent(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
