/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Diagnostics.CodeAnalysis;

namespace miniblocks.API.Proxies;

public class LeasedProxy<T> : Proxy<T>, ILeasedProxy<T>
{
    private readonly Proxy<T> source;

    private readonly T valueBeforeLease;
    private readonly bool disabledBeforeLease, revertValueOnReturn;

    private bool hasBeenReturned;

    public override T Value
    {
        get => base.Value;
        set
        {
            if (source != null)
                validate();

            if (Comparer.Equals(Value, value)) return;

            SetCurrentValue(base.Value, value, true);
        }
    }

    public override T Default
    {
        get => base.Default;
        set
        {
            if (source != null)
                validate();

            if (Comparer.Equals(Default, value)) return;

            SetDefaultValue(base.Default, value, true);
        }
    }

        public override bool Disabled
        {
            get => base.Disabled;
            set
            {
                if (source != null)
                    validate();

                if (Disabled == value) return;

                SetDisabled(value, true);
            }
        }

    internal LeasedProxy([NotNull] Proxy<T> source, bool revertOnReturn)
    {
        ConnectTo(source);

        this.source = source ?? throw new ArgumentNullException(nameof(source));

        if (revertOnReturn)
        {
            revertValueOnReturn = true;
            valueBeforeLease = Value;
        }

        disabledBeforeLease = Disabled;
        Disabled = true;
    }

    private LeasedProxy(T defaultValue = default!)
        : base(defaultValue)
    {
    }

    public bool Return()
    {
        if (hasBeenReturned)
            return false;

        if (source == null)
            throw new InvalidOperationException($"Must {nameof(Return)} from original leased source");

        DisconnectAll();
        return true;
    }

    protected override Proxy<T> CreateInstance()
        => new LeasedProxy<T>();

    internal override void DisconnectAllInternal()
    {
        if (source != null && !hasBeenReturned)
        {
            if (revertValueOnReturn)
                Value = valueBeforeLease;

            Disabled = disabledBeforeLease;

            source.EndLease(this);

            hasBeenReturned = true;
        }

        base.DisconnectAllInternal();
    }

    private void validate()
    {
        if (source != null && hasBeenReturned)
            throw new InvalidOperationException($"Cannot perform operations on a {nameof(LeasedProxy<T>)} that has been {nameof(Return)}ed.");
    }

}
