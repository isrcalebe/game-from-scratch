/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

namespace miniblocks.API.Timing;

public interface IFrameBasedClock : IClock
{
    FrameTimeData Data { get; }

    double ElapsedFrameTime { get; }

    double FramesPerSecond { get; }

    void ProcessFrame();
}
