/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

#nullable enable

namespace miniblocks.API.Timing;

public interface ISourceChangeableClock : IClock
{
    IClock? Source { get; }

    void ChangeSource(IClock? source);
}
