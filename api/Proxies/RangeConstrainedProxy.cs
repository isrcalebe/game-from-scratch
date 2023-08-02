/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Proxies;

public abstract class RangeConstrainedProxy<T> : Proxy<T>
{
    private T minValue, maxValue;

    public event Action<T> MinValueChanged;
    public event Action<T> MaxValueChanged;

    public T MinValue
    {
        get => minValue;
        set
        {
            if (Comparer.Equals(value, minValue)) return;

            SetMinValue(value, true, this);
        }
    }

    public T MaxValue
    {
        get => maxValue;
        set
        {
            if (Comparer.Equals(value, maxValue)) return;

            SetMaxValue(value, true, this);
        }
    }

    public override T Value
    {
        get => base.Value;
        set => setValue(value);
    }

    protected abstract T DefaultMinValue { get; }

    protected abstract T DefaultMaxValue { get; }

    public bool HasDefinedRange => !Comparer.Equals(MinValue, DefaultMinValue)
                                || !Comparer.Equals(MaxValue, DefaultMaxValue);

    internal void SetMinValue(T value, bool updateCurrentValue, RangeConstrainedProxy<T> source)
    {
        minValue = value;
        TriggerMinValueChanged(source);

        if (updateCurrentValue)
            setValue(Value);
    }

    internal void SetMaxValue(T value, bool updateCurrentValue, RangeConstrainedProxy<T> source)
    {
        maxValue = value;
        TriggerMaxValueChanged(source);

        if (updateCurrentValue)
            setValue(Value);
    }

    public override void TriggerChange()
    {
        base.TriggerChange();

        TriggerMinValueChanged(this, false);
        TriggerMaxValueChanged(this, false);
    }

    protected void TriggerMinValueChanged(RangeConstrainedProxy<T> source = null, bool propagate = true)
    {
        var beforePropagation = minValue;

        if (propagate && Connections != null)
        {
            foreach (var proxy in Connections)
            {
                if (proxy == source) continue;

                if (proxy is RangeConstrainedProxy<T> connection)
                    connection.SetMinValue(minValue, false, this);
            }
        }

        if (Comparer.Equals(beforePropagation, minValue))
            MinValueChanged?.Invoke(minValue);
    }

    protected void TriggerMaxValueChanged(RangeConstrainedProxy<T> source = null, bool propagate = true)
    {
        var beforePropagation = maxValue;

        if (propagate && Connections != null)
        {
            foreach (var proxy in Connections)
            {
                if (proxy == source) continue;

                if (proxy is RangeConstrainedProxy<T> connection)
                    connection.SetMaxValue(maxValue, false, this);
            }
        }

        if (Comparer.Equals(beforePropagation, maxValue))
            MaxValueChanged?.Invoke(maxValue);
    }

    public override void ConnectTo(Proxy<T> other)
    {
        if (other is RangeConstrainedProxy<T> proxy)
        {
            if (!IsValidRange(proxy.MinValue, proxy.MaxValue))
                throw new ArgumentOutOfRangeException(
                    nameof(other), $"The target proxy has specified an invalid range of [{proxy.MinValue} - {proxy.MaxValue}]."
                );
        }

        base.ConnectTo(other);
    }

    public override void DisconnectEvents()
    {
        base.DisconnectEvents();

        MinValueChanged = null;
        MaxValueChanged = null;
    }

    public new RangeConstrainedProxy<T> GetBoundCopy()
        => (RangeConstrainedProxy<T>)base.GetBoundCopy();

    public new RangeConstrainedProxy<T> GetUnboundCopy()
        => (RangeConstrainedProxy<T>)base.GetUnboundCopy();

    protected abstract T Clamp(T value, T minimum, T maximum);

    protected abstract bool IsValidRange(T minimum, T maximum);

    private void setValue(T value)
        => base.Value = Clamp(value, minValue, maxValue);
}
