/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace miniblocks.API.Collections;

public class LockedWeakList<T> : IWeakList<T>, IEnumerable<T>
    where T : class
{
    private readonly WeakList<T> list;

    public LockedWeakList()
    {
        list = new();
    }

    public void Add(T item)
    {
        lock (list)
            list.Add(item);
    }

    public void Add(WeakReference<T> weakReference)
    {
        lock (list)
            list.Add(weakReference);
    }

    public bool Remove(T item)
    {
        lock (list)
            return list.Remove(item);
    }

    public bool Remove(WeakReference<T> weakReference)
    {
        lock (list)
            return list.Remove(weakReference);
    }

    public void RemoveAt(int index)
    {
        lock (list)
            list.RemoveAt(index);
    }

    public bool Contains(T item)
    {
        lock (list)
            return list.Contains(item);
    }

    public bool Contains(WeakReference<T> weakReference)
    {
        lock (list)
            return list.Contains(weakReference);
    }

    public void Clear()
    {
        lock (list)
            list.Clear();
    }

    public Enumerator GetEnumerator()
        => new(list);

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public struct Enumerator : IEnumerator<T>
    {
        private readonly WeakList<T> list;

        private WeakList<T>.ValidItemsEnumerator enumerator;

        private readonly bool locker;

        public readonly T Current => enumerator.Current;

        readonly object IEnumerator.Current => Current;

        internal Enumerator(WeakList<T> list)
        {
            this.list = list;

            locker = false;
            Monitor.Enter(list, ref locker);

            enumerator = list.GetEnumerator();
        }

        public bool MoveNext() => enumerator.MoveNext();

        public void Reset() => enumerator.Reset();

        public readonly void Dispose()
        {
            if (locker)
                Monitor.Exit(list);
        }
    }
}
