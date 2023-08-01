/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Proxies;

public interface ILeasedProxy : IProxy
{
    bool Return();
}

public interface ILeasedProxy<T> : ILeasedProxy, IProxy<T>
{
}
