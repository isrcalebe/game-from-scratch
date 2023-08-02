/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Proxies;

public class NonNullableProxy<T> : Proxy<T>
    where T : class
{
    public NonNullableProxy(T defaultValue)
    {
        ArgumentNullException.ThrowIfNull(defaultValue);

        Value = Default = defaultValue;
    }

    private NonNullableProxy()
    {
    }

    public override T Value
    {
        get => base.Value;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), $"Cannot set {nameof(Value)} of a {nameof(NonNullableProxy<T>)} to null.");

            base.Value = value;
        }
    }

    protected override Proxy<T> CreateInstance()
        => new NonNullableProxy<T>();
}
