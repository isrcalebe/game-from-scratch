/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Globalization;
using System.Numerics;
using miniblocks.API.Utilities;

namespace miniblocks.API.Proxies;

public class ProxyNumber<T> : RangeConstrainedProxy<T>, IProxyNumber<T>
    where T : struct, IComparable<T>, IEquatable<T>, IConvertible
{
    private T precision;

    public event Action<T> PrecisionChanged;

    public T Precision
    {
        get => precision;
        set
        {
            if (precision.Equals(value)) return;

            if (value.CompareTo(default) <= 0)
                throw new ArgumentOutOfRangeException(nameof(Precision), value, "Must be greater than 0.");

            SetPrecision(value, true, this);
        }
    }

    public override T Value
    {
        get => base.Value;
        set => setValue(value);
    }

    protected override T DefaultMinValue
    {
        get
        {

            if (typeof(T) == typeof(sbyte))
                return (T)(object)sbyte.MinValue;
            if (typeof(T) == typeof(byte))
                return (T)(object)byte.MinValue;
            if (typeof(T) == typeof(short))
                return (T)(object)short.MinValue;
            if (typeof(T) == typeof(ushort))
                return (T)(object)ushort.MinValue;
            if (typeof(T) == typeof(int))
                return (T)(object)int.MinValue;
            if (typeof(T) == typeof(uint))
                return (T)(object)uint.MinValue;
            if (typeof(T) == typeof(long))
                return (T)(object)long.MinValue;
            if (typeof(T) == typeof(ulong))
                return (T)(object)ulong.MinValue;
            if (typeof(T) == typeof(float))
                return (T)(object)float.MinValue;

            return (T)(object)double.MinValue;
        }
    }

    protected override T DefaultMaxValue
    {
        get
        {

            if (typeof(T) == typeof(sbyte))
                return (T)(object)sbyte.MaxValue;
            if (typeof(T) == typeof(byte))
                return (T)(object)byte.MaxValue;
            if (typeof(T) == typeof(short))
                return (T)(object)short.MaxValue;
            if (typeof(T) == typeof(ushort))
                return (T)(object)ushort.MaxValue;
            if (typeof(T) == typeof(int))
                return (T)(object)int.MaxValue;
            if (typeof(T) == typeof(uint))
                return (T)(object)uint.MaxValue;
            if (typeof(T) == typeof(long))
                return (T)(object)long.MaxValue;
            if (typeof(T) == typeof(ulong))
                return (T)(object)ulong.MaxValue;
            if (typeof(T) == typeof(float))
                return (T)(object)float.MaxValue;

            return (T)(object)double.MinValue;
        }
    }

    protected virtual T DefaultPrecision
    {
        get
        {
            if (typeof(T) == typeof(sbyte))
                return (T)(object)(sbyte)1;
            if (typeof(T) == typeof(byte))
                return (T)(object)(byte)1;
            if (typeof(T) == typeof(short))
                return (T)(object)(short)1;
            if (typeof(T) == typeof(ushort))
                return (T)(object)(ushort)1;
            if (typeof(T) == typeof(int))
                return (T)(object)1;
            if (typeof(T) == typeof(uint))
                return (T)(object)1U;
            if (typeof(T) == typeof(long))
                return (T)(object)1L;
            if (typeof(T) == typeof(ulong))
                return (T)(object)1UL;
            if (typeof(T) == typeof(float))
                return (T)(object)float.Epsilon;

            return (T)(object)double.Epsilon;
        }
    }

    public bool IsInteger =>
        typeof(T) != typeof(float) &&
        typeof(T) != typeof(double);

    public override bool IsDefault
    {
        get
        {
            if (typeof(T) == typeof(double))
                return PrecisionComparer.AlmostEquals((double)(object)Value, (double)(object)Default, (double)(object)Precision / 2);

            if (typeof(T) == typeof(float))
                return PrecisionComparer.AlmostEquals((float)(object)Value, (float)(object)Default, (float)(object)Precision / 2);

            return base.IsDefault;
        }
    }

    public override void TriggerChange()
    {
        base.TriggerChange();

        TriggerPrecisionChange(this, false);
    }

    protected void TriggerPrecisionChange(ProxyNumber<T> source = null, bool propagate = true)
    {
        var beforePropagation = precision;

        if (propagate && Connections != null)
        {
            foreach (var proxy in Connections)
            {
                if (proxy == source) continue;

                if (proxy is ProxyNumber<T> connection)
                    connection.SetPrecision(precision, false, this);
            }
        }

        if (Comparer.Equals(beforePropagation, precision))
            PrecisionChanged?.Invoke(precision);
    }

    public override void ConnectTo(Proxy<T> other)
    {
        if (other is ProxyNumber<T> proxy)
            proxy.Precision = Precision;

        base.ConnectTo(other);
    }

    public override void DisconnectEvents()
    {
        base.DisconnectEvents();

        PrecisionChanged = null;
    }

    public void Set<TNewValue>(TNewValue value)
        where TNewValue : struct, IFormattable, IConvertible, IComparable<TNewValue>, IEquatable<TNewValue>
    {
        if (typeof(T) == typeof(byte))
                ((ProxyNumber<byte>)(object)this).Value = value.ToByte(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(sbyte))
                ((ProxyNumber<sbyte>)(object)this).Value = value.ToSByte(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(ushort))
                ((ProxyNumber<ushort>)(object)this).Value = value.ToUInt16(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(short))
                ((ProxyNumber<short>)(object)this).Value = value.ToInt16(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(uint))
                ((ProxyNumber<uint>)(object)this).Value = value.ToUInt32(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(int))
                ((ProxyNumber<int>)(object)this).Value = value.ToInt32(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(ulong))
                ((ProxyNumber<ulong>)(object)this).Value = value.ToUInt64(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(long))
                ((ProxyNumber<long>)(object)this).Value = value.ToInt64(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(float))
                ((ProxyNumber<float>)(object)this).Value = value.ToSingle(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(double))
                ((ProxyNumber<double>)(object)this).Value = value.ToDouble(NumberFormatInfo.InvariantInfo);
    }

    public void Add<TNewValue>(TNewValue value)
        where TNewValue : struct, IFormattable, IConvertible, IComparable<TNewValue>, IEquatable<TNewValue>
    {
        if (typeof(T) == typeof(byte))
                ((ProxyNumber<byte>)(object)this).Value += value.ToByte(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(sbyte))
                ((ProxyNumber<sbyte>)(object)this).Value += value.ToSByte(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(ushort))
                ((ProxyNumber<ushort>)(object)this).Value += value.ToUInt16(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(short))
                ((ProxyNumber<short>)(object)this).Value += value.ToInt16(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(uint))
                ((ProxyNumber<uint>)(object)this).Value += value.ToUInt32(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(int))
                ((ProxyNumber<int>)(object)this).Value += value.ToInt32(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(ulong))
                ((ProxyNumber<ulong>)(object)this).Value += value.ToUInt64(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(long))
                ((ProxyNumber<long>)(object)this).Value += value.ToInt64(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(float))
                ((ProxyNumber<float>)(object)this).Value += value.ToSingle(NumberFormatInfo.InvariantInfo);
            else if (typeof(T) == typeof(double))
                ((ProxyNumber<double>)(object)this).Value += value.ToDouble(NumberFormatInfo.InvariantInfo);
    }

    public void SetProportional(float amount, float snap = 0.0f)
    {
        var min = MinValue.ToDouble(NumberFormatInfo.InvariantInfo);
        var max = MaxValue.ToDouble(NumberFormatInfo.InvariantInfo);
        var value = min + (max - min) * amount;

        if (snap > 0.0f)
            value = Math.Round(value / snap) * snap;

        Set(value);
    }

    internal void SetPrecision(T value, bool updateCurrentValue, ProxyNumber<T> source)
    {
        precision = value;
        TriggerPrecisionChange(source);

        if (updateCurrentValue)
            setValue(Value);
    }

    private void setValue(T value)
    {
        if (Precision.CompareTo(DefaultPrecision) > 0)
        {
            var clamped = Clamp(value, MinValue, MaxValue).ToDouble(NumberFormatInfo.InvariantInfo);
            clamped = Math.Round(clamped / Precision.ToDouble(NumberFormatInfo.InvariantInfo)) * Precision.ToDouble(NumberFormatInfo.InvariantInfo);

            base.Value = (T)Convert.ChangeType(clamped, typeof(T), CultureInfo.InvariantCulture);
        }
        else
            base.Value = value;
    }

    protected sealed override T Clamp(T value, T minimum, T maximum)
        => max(minimum, min(maximum, value));

    protected sealed override bool IsValidRange(T minimum, T maximum)
        => minimum.CompareTo(maximum) <= 0;

    protected override Proxy<T> CreateInstance()
        => new ProxyNumber<T>();

    private static T max(T lhs, T rhs)
        => lhs.CompareTo(rhs) > 0 ? lhs : rhs;

    private static T min(T lhs, T rhs)
        => lhs.CompareTo(rhs) > 0 ? rhs : lhs;

    public new ProxyNumber<T> GetBoundCopy()
        => (ProxyNumber<T>)base.GetBoundCopy();

    public new ProxyNumber<T> GetUnboundCopy()
        => (ProxyNumber<T>)base.GetUnboundCopy();

    IProxyNumber<T> IProxyNumber<T>.GetBoundCopy()
        => GetBoundCopy();
}
