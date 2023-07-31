/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;

namespace miniblocks.API.Collections;

public partial class WeakList<T> : IWeakList<T>, IEnumerable<T>
    where T : class
{
    private const int opportunistic_trim_threshold = 30;

    private int listStart, listEnd;
    private int countChangesSinceTrim;
    private readonly List<InvalidatableWeakReference> list = new();

    public void Add(T item)
        => add(new InvalidatableWeakReference(item));

    public void Add(WeakReference<T> reference)
        => add(new InvalidatableWeakReference(reference));

    public bool Remove(T item)
    {
        var hashCode = EqualityComparer<T>.Default.GetHashCode(item);

        for (var index = listStart; index < listEnd; index++)
        {
            var reference = list[index].Reference;

            if (reference == null) continue;
            if (list[index].ObjectHashCode != hashCode) continue;
            if (!reference.TryGetTarget(out var target) || target != item) continue;

            RemoveAt(index - listStart);
            return true;
        }

        return false;
    }

    public bool Remove(WeakReference<T> reference)
    {
        for (int index = listStart; index < listEnd; index++)
        {
            if (list[index].Reference != reference) continue;

            RemoveAt(index - listStart);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        index += listStart;

        if (index < listStart || index >= listEnd)
            throw new ArgumentOutOfRangeException(nameof(index));

        list[index] = default;

        if (index == listStart)
            listStart++;
        else if (index == listEnd - 1)
            listEnd--;

        countChangesSinceTrim++;
    }

    public bool Contains(T item)
    {
        var hashCode = EqualityComparer<T>.Default.GetHashCode(item);

        for (var index = listStart; index < listEnd; index++)
        {
            var reference = list[index].Reference;

            if (reference == null) continue;
            if (list[index].ObjectHashCode != hashCode) continue;
            if (!reference.TryGetTarget(out var target) || target != item) continue;

            return true;
        }

        return false;
    }

    public bool Contains(WeakReference<T> reference)
    {
        for (int i = listStart; i < listEnd; i++)
        {
            if (list[i].Reference == reference)
                return true;
        }

        return false;
    }

    public void Clear()
    {
        listStart = listEnd = 0;
        countChangesSinceTrim = list.Count;
    }

    public ValidItemsEnumerator GetEnumerator()
    {
        trim();
        return new ValidItemsEnumerator(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private void add(in InvalidatableWeakReference reference)
    {
        if (countChangesSinceTrim > opportunistic_trim_threshold)
            trim();

        if (listEnd < list.Count)
        {
            list[listEnd] = reference;
            countChangesSinceTrim--;
        }
        else
        {
            list.Add(reference);
            countChangesSinceTrim++;
        }

        listEnd++;
    }

    private void trim()
    {
        list.RemoveRange(listEnd, list.Count - listEnd);
        list.RemoveRange(0, listStart);

        list.RemoveAll(item => item.Reference == null
                            || !item.Reference.TryGetTarget(out _));

        listStart = 0;
        listEnd = list.Count;

        countChangesSinceTrim = 0;
    }

    private readonly struct InvalidatableWeakReference
    {
        public readonly WeakReference<T>? Reference;

        public readonly int ObjectHashCode;

        public InvalidatableWeakReference(T reference)
        {
            Reference = new WeakReference<T>(reference);
            ObjectHashCode = EqualityComparer<T>.Default.GetHashCode(reference);
        }
        public InvalidatableWeakReference(WeakReference<T> weakReference)
        {
            Reference = weakReference;
            ObjectHashCode = !weakReference.TryGetTarget(out var target) ? 0 : EqualityComparer<T>.Default.GetHashCode(target);
        }
    }

    public struct ValidItemsEnumerator : IEnumerator<T>
    {
        private readonly WeakList<T> weakList;
        private int currentItemIndex;

        public T Current { get; private set; }

        readonly object IEnumerator.Current => Current;

        internal ValidItemsEnumerator(WeakList<T> weakList)
        {
            this.weakList = weakList;
            currentItemIndex = weakList.listStart - 1;
            Current = default!;
        }

        public bool MoveNext()
        {
            while (true)
            {
                ++currentItemIndex;

                if (currentItemIndex >= weakList.listEnd)
                    return false;

                var reference = weakList.list[currentItemIndex].Reference;

                if (reference == null || !reference.TryGetTarget(out var item)) continue;

                Current = item;
                return true;
            }
        }

        public void Reset()
        {
            currentItemIndex = weakList.listStart - 1;
            Current = default!;
        }

        public void Dispose()
        {
            Current = default!;
        }
    }
}
