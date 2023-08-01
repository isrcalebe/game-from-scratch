/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;

namespace miniblocks.API.Proxies;

public interface ICanBeDisabled
{
    event Action<bool> DisabledChanged;

    void ConnectDisabledChanged(Action<bool> onChange, bool invokeImediately = false);

    bool Disabled { get; }
}
