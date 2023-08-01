/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Timing;

public interface IClock
{
    bool IsRunning { get; }

    double CurrentTime { get; }

    double Rate { get; }
}
