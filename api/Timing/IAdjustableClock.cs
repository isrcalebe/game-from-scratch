/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Timing;

public interface IAdjustableClock : IClock
{
    new double Rate { get; set; }

    void Start();

    void Stop();

    bool Seek(double cursor);

    void Reset();

    void ResetSpeedAdjustments();
}
