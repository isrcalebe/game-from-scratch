/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

#nullable enable
#pragma warning disable CS8618

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using miniblocks.API.Collections;
using miniblocks.API.Extensions.ObjectExtensions;
using miniblocks.API.Extensions.TypeExtensions;
using miniblocks.API.Proxies.Events;

namespace miniblocks.API.Proxies;

public class Proxy<T> : IProxy<T>, IProxy, ICanBeParsed
{
    private T currentValue, defaultValue;
    private bool proxyDisabled;

    private bool isLeased => leasedProxy != null;

    private LeasedProxy<T>? leasedProxy;

    private WeakReference<Proxy<T>>? referenceInstance;
    private WeakReference<Proxy<T>> reference => referenceInstance ??= new(this);

    protected EqualityComparer<T> Comparer;

    public event Action<ValueChangedEventArgs<T>>? ValueChanged;

    public event Action<ValueChangedEventArgs<T>>? DefaultChanged;

    public event Action<bool>? DisabledChanged;

    protected LockedWeakList<Proxy<T>>? Connections { get; private set; }

    public virtual T Value
    {
        get => currentValue;
        set
        {
            if (Disabled)
                throw new InvalidOperationException($"Can't set value to \"{value}\" because the proxy is disabled.");

            if (Comparer.Equals(currentValue, value)) return;

            SetCurrentValue(currentValue, value);
        }
    }

    public virtual T Default
    {
        get => defaultValue;
        set
        {
            if (Disabled)
                throw new InvalidOperationException($"Can't set default value to \"{value}\" because the proxy is disabled.");

            if (Comparer.Equals(defaultValue, value)) return;

            SetDefaultValue(defaultValue, value);
        }
    }

    public virtual bool Disabled
    {
        get => proxyDisabled;
        set
        {
            throwIfLeased();

            if (proxyDisabled == value) return;

            SetDisabled(value);
        }
    }

    public virtual bool IsDefault => Comparer.Equals(currentValue, Default);

    public IProxy<T> Connection
    {
        set => ((IProxy<T>)this).ConnectTo(value);
    }

    public Proxy(T defaultValue = default!)
    {
        Comparer = CreateComparer();
        currentValue = Default = defaultValue;
    }

    public virtual void Parse(object? input)
    {
        switch (input)
        {
            case T type:
                Value = type;
                break;

            case null:
                if (typeof(T).IsNullable() || typeof(T).IsClass)
                {
                    Value = default!;
                    break;
                }

                throw new ArgumentNullException(nameof(input));
            case IProxy:
                if (input is not IProxy<T> proxy)
                    throw new ArgumentException($"Expected proxy of type {nameof(IProxy)}<{typeof(T)}>, got {input.GetType()}", nameof(input));

                Value = proxy.Value;
                break;
            default:
                if (input is string strInput && string.IsNullOrEmpty(strInput))
                {
                    if (typeof(T).IsNullable() || typeof(T).IsClass)
                    {
                        Value = default!;
                        break;
                    }
                }

                var underlyingType = typeof(T).GetUnderlyingNullableType() ?? typeof(T);
                if (underlyingType.IsEnum)
                    Value = (T)Enum.Parse(underlyingType, input.ToString().AsNonNull());
                else
                    Value = (T)Convert.ChangeType(input, underlyingType, CultureInfo.InvariantCulture);
                break;
        }
    }

    public void ConnectValueChanged(Action<ValueChangedEventArgs<T>> onChange, bool invokeImmediately = false)
    {
        ValueChanged += onChange;

        if (invokeImmediately)
            onChange?.Invoke(new ValueChangedEventArgs<T>(Value, Value));
    }

    public void ConnectDisabledChanged(Action<bool> onChange, bool invokeImmediately = false)
    {
        DisabledChanged += onChange;

        if (invokeImmediately)
            onChange?.Invoke(Disabled);
    }

    public virtual void TriggerChange()
    {
        TriggerValueChange(currentValue, this, false);
        TriggerDisabledChange(this, false);
    }

    protected void TriggerValueChange(T previousValue, Proxy<T> source, bool propagate = true, bool bypass = false)
    {
        var beforePropagation = currentValue;

        if (propagate && Connections != null)
        {
            foreach (var proxy in Connections)
            {
                if (proxy == source) continue;

                proxy.SetCurrentValue(previousValue, currentValue, bypass, this);
            }
        }

        if (Comparer.Equals(beforePropagation, currentValue))
            ValueChanged?.Invoke(new ValueChangedEventArgs<T>(previousValue, currentValue));
    }

    protected void TriggerDefaultChange(T previousValue, Proxy<T> source, bool propagate = true, bool bypass = false)
    {
        var beforePropagation = defaultValue;

        if (propagate && Connections != null)
        {
            foreach (var proxy in Connections)
            {
                if (proxy == source) continue;

                proxy.SetDefaultValue(previousValue, defaultValue, bypass, this);
            }
        }

        if (Comparer.Equals(beforePropagation, defaultValue))
            DefaultChanged?.Invoke(new ValueChangedEventArgs<T>(previousValue, defaultValue));
    }

    protected void TriggerDisabledChange(Proxy<T> source, bool propagate = true, bool bypass = false)
    {
        var beforePropagation = proxyDisabled;

        if (propagate && Connections != null)
        {
            foreach (var proxy in Connections)
            {
                if (proxy == source) continue;

                proxy.SetDisabled(proxyDisabled, bypass, this);
            }
        }

        if (beforePropagation == proxyDisabled)
            DisabledChanged?.Invoke(proxyDisabled);
    }

    protected virtual EqualityComparer<T> CreateComparer()
        => EqualityComparer<T>.Default;

    public virtual void DisconnectEvents()
    {
        ValueChanged = null;
        DefaultChanged = null;
        DisabledChanged = null;
    }

    public virtual void DisconnectProxies()
    {
        if (Connections == null) return;

        foreach (var proxy in Connections.ToArray())
            DisconnectFrom(proxy);
    }

    public virtual void DisconnectFrom(IDisconnectable disconnectable)
    {
        if (disconnectable is not Proxy<T> proxy)
            throw new InvalidOperationException();

        removeWeakReference(proxy.reference);
        proxy.removeWeakReference(reference);
    }

    public void DisconnectAll()
        => DisconnectAllInternal();

    public sealed override string ToString()
        => ToString(null, CultureInfo.CurrentCulture);

    public virtual string ToString(string? format, IFormatProvider formatProvider)
        => string.Format(formatProvider, $"{{0:{format}}}", Value);


    internal virtual void DisconnectAllInternal()
    {
        if (isLeased)
            leasedProxy?.Return();

        DisconnectEvents();
        DisconnectProxies();
    }

    internal void SetDisabled(bool value, bool bypass = false, Proxy<T>? source = null)
    {
        if (!bypass)
            throwIfLeased();

        proxyDisabled = value;
        TriggerDisabledChange(source ?? this, true, bypass);
    }

    internal void SetCurrentValue(T previousValue, T newValue, bool bypass = false, Proxy<T>? source = null)
    {
        currentValue = newValue;
        TriggerValueChange(previousValue, source ?? this, true, bypass);
    }

    internal void SetDefaultValue(T previousValue, T newValue, bool bypass = false, Proxy<T>? source = null)
    {
        defaultValue = newValue;
        TriggerDefaultChange(previousValue, source ?? this, true, bypass);
    }

    public virtual void CopyTo(Proxy<T> proxy)
    {
        proxy.Value = Value;
        proxy.Default = Default;
        proxy.Disabled = Disabled;
    }

    public virtual void ConnectTo(Proxy<T> proxy)
    {
        if (Connections?.Contains(proxy.reference) == true)
            throw new ArgumentException("An already connected proxy cannot be connected again.");

        proxy.CopyTo(this);

        addWeakReference(proxy.reference);
        proxy.addWeakReference(reference);
    }

    public Proxy<T> GetUnboundCopy()
    {
        var newProxy = CreateInstance();
        ConnectTo(newProxy);

        return newProxy;
    }

    public Proxy<T> GetBoundCopy()
        => IProxy.GetBoundCopyImplementation(this);

    protected virtual Proxy<T> CreateInstance()
        => CreateInstance();

    IProxy IProxy.CreateInstance()
        => CreateInstance();

    IProxy IProxy.GetBoundCopy()
        => GetBoundCopy();

    IProxy<T> IProxy<T>.GetBoundCopy()
        => GetBoundCopy();

    void IProxy.ConnectTo(IProxy other)
    {
        if (other is not Proxy<T> proxy)
            throw new InvalidCastException($"Can't connect to a proxy of type {other.GetType()} from a proxy of type {GetType()}.");

        ConnectTo(proxy);
    }

    void IProxy<T>.ConnectTo(IProxy<T> other)
    {
        if (other is not Proxy<T> proxy)
            throw new InvalidCastException($"Can't connect to a proxy of type {other.GetType()} from a proxy of type {GetType()}.");

        ConnectTo(proxy);
    }

    public LeasedProxy<T> BeginLease(bool revertValueOnReturn)
    {
        if (checkForLease(this))
            throw new InvalidOperationException("Attempted to lease a proxy that is already in a leased state.");

        return leasedProxy = new LeasedProxy<T>(this, revertValueOnReturn);
    }

    internal void EndLease(ILeasedProxy<T> returnedProxy)
    {
        if (!isLeased)
            throw new InvalidOperationException("Attempted to end a lease without beginning one.");

        if (returnedProxy != leasedProxy)
            throw new InvalidOperationException("Attempted to end a lease but returned a different proxy to the one used to start the lease.");

        leasedProxy = null;
    }

    private void addWeakReference(WeakReference<Proxy<T>> reference)
    {
        Connections ??= new LockedWeakList<Proxy<T>>();
        Connections.Add(reference);
    }

    private void removeWeakReference(WeakReference<Proxy<T>> reference)
        => Connections?.Remove(reference);

    private void throwIfLeased()
    {
        if (isLeased)
            throw new InvalidOperationException($"Cannot perform this operation on a {nameof(Proxy<T>)} that is currently in a leased state.");
    }

    private bool checkForLease(Proxy<T> source)
    {
        if (isLeased)
            return true;

        if (Connections == null)
            return false;

        bool found = false;

        foreach (var proxy in Connections)
        {
            if (proxy != source)
                found |= proxy.checkForLease(this);
        }

        return found;
    }
}
