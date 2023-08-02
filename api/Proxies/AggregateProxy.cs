/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Collections.Generic;
using System.Linq;
using miniblocks.API.Proxies.Events;

namespace miniblocks.API.Proxies;

public class AggregateProxy<T>
{
    private readonly Func<T, T, T> aggregateFunction;
    private readonly Proxy<T> result;

    private readonly T initialValue;

    private readonly List<WeakReferencePair> sourceMapping;

    public IProxy<T> Result => result;

    public AggregateProxy()
    {
        sourceMapping = new List<WeakReferencePair>();
    }

    public void AddSource(IProxy<T> proxy)
    {
        lock (sourceMapping)
        {
            if (findExistingPair(proxy) != null)
                return;

            var boundCopy = proxy.GetBoundCopy();

            sourceMapping.Add(new WeakReferencePair(new WeakReference<IProxy<T>>(proxy), boundCopy));
            boundCopy.ConnectValueChanged(recalculateAggregate, true);
        }
    }

    public void RemoveSource(IProxy<T> proxy)
    {
        lock (sourceMapping)
        {
            var weak = findExistingPair(proxy);

            if (weak != null)
            {
                weak.BoundCopy.DisconnectAll();
                sourceMapping.Remove(weak);
            }

            recalculateAggregate(ValueChangedEvent<T>.Default);
        }
    }

    public void RemoveAllSources()
    {
        lock (sourceMapping)
        {
            foreach (var mapping in sourceMapping.ToArray())
            {
                if (mapping.WeakReference.TryGetTarget(out var proxy))
                    RemoveSource(proxy);
            }
        }
    }

    private WeakReferencePair findExistingPair(IProxy<T> proxy)
        => sourceMapping
            .FirstOrDefault(pair => pair.WeakReference.TryGetTarget(out var target) && target == proxy);

    private void recalculateAggregate(ValueChangedEvent<T> e)
    {
        var calculated = initialValue;

        lock (sourceMapping)
        {
            for (var index = 0; index < sourceMapping.Count; index++)
            {
                var pair = sourceMapping[index];

                if (!pair.WeakReference.TryGetTarget(out _))
                    sourceMapping.RemoveAt(index--);
                else
                    calculated = aggregateFunction(calculated, pair.BoundCopy.Value);
            }
        }

        result.Value = calculated;
    }

    private class WeakReferencePair
    {
        public readonly WeakReference<IProxy<T>> WeakReference;
        public readonly IProxy<T> BoundCopy;

        public WeakReferencePair(WeakReference<IProxy<T>> weakReference, IProxy<T> boundCopy)
        {
            WeakReference = weakReference;
            BoundCopy = boundCopy;
        }
    }
}
