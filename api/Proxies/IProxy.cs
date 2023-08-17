/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System;
using miniblocks.API.Extensions.TypeExtensions;
using miniblocks.API.Proxies.Events;

namespace miniblocks.API.Proxies;

public interface IProxy : IDisconnectable, ICanBeDisabled, IHasDefaultValue
{
    sealed IProxy Connection
    {
        set => ConnectTo(value);
    }

    void ConnectTo(IProxy proxy);

    IProxy GetBoundCopy();

    protected IProxy CreateInstance();

    protected static T GetBoundCopyImplementation<T>(T source)
        where T : IProxy
    {
        var copy = source.CreateInstance();

        if (copy.GetType() != source.GetType())
            throw new InvalidOperationException($"The type of the copy ({copy.GetType().ReadableName()}) is not the same as the type of the source ({source.GetType().ReadableName()})."
                                                + $"Override {source.GetType().ReadableName()}.{nameof(CreateInstance)}() for {nameof(GetBoundCopy)}() to function properly.");

        copy.ConnectTo(source);

        return (T)copy;
    }
}

public interface IProxy<T> : IDisconnectable
{
    event Action<ValueChangedEventArgs<T>>? ValueChanged;

    T Value { get; }

    T Default { get; }

    IProxy<T> Connection
    {
        set => ConnectTo(value);
    }

    void ConnectTo(IProxy<T> proxy);

    void ConnectValueChanged(Action<ValueChangedEventArgs<T>> onChange, bool invokeImmediately = false);

    IProxy<T> GetBoundCopy();
}
