/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Proxies;

public interface IDisconnectable
{
    void DisconnectEvents();

    void DisconnectProxies();

    void DisconnectAll();

    void DisconnectFrom(IDisconnectable disconnectable);
}
