/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Collections;

public interface IWeakList<T>
    where T : class
{
    void Add(T item);

    void Add(WeakReference<T> weakReference);

    bool Remove(T item);

    bool Remove(WeakReference<T> weakReference);

    void RemoveAt(int index);

    bool Contains(T item);

    bool Contains(WeakReference<T> weakReference);

    void Clear();
}
