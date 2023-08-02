/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Proxies;

public interface IProxyNumber<T> : IProxy<T>
    where T : struct, IComparable<T>, IEquatable<T>
{
    event Action<T> PrecisionChanged;

    event Action<T> MinValueChanged;

    event Action<T> MaxValueChanged;

    T Precision { get; }

    T MinValue { get; }

    T MaxValue { get; }

    bool IsInteger { get; }

    new IProxyNumber<T> GetBoundCopy();
}
