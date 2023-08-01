/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Diagnostics;
using miniblocks.API.Extensions.TypeExtensions;

namespace miniblocks.API.Timing;

public class StopwatchClock : Stopwatch, IAdjustableClock
{
    private double seekOffset;
    private double rateChangeUsed;
    private double rateChangeAccumulated;

    private double rate = 1;

    private double stopwatchMilliseconds => (double)ElapsedTicks / Frequency * 1000;
    private double stopwatchCurrentTime => (stopwatchMilliseconds - rateChangeUsed) * rate + rateChangeAccumulated;

    public double CurrentTime
        => stopwatchCurrentTime + seekOffset;

    public double Rate
    {
        get => rate;
        set
        {
            if (rate == value) return;

            rateChangeAccumulated += (stopwatchMilliseconds - rateChangeUsed) * rate;
            rateChangeUsed = stopwatchMilliseconds;

            rate = value;
        }
    }

    public StopwatchClock(bool start = false)
    {
        if (start)
            Start();
    }

    public bool Seek(double cursor)
    {
        seekOffset = cursor - stopwatchCurrentTime;
        return true;
    }

    public void ResetSpeedAdjustments()
        => Rate = 1;

    public new void Reset()
    {
        resetAccumulatedRate();
        base.Reset();
    }

    public new void Restart()
    {
        resetAccumulatedRate();
        base.Restart();
    }

    public override string ToString()
        => $@"{GetType().ReadableName()} ({Math.Truncate(CurrentTime)}ms)";

    private void resetAccumulatedRate()
    {
        rateChangeAccumulated = 0;
        rateChangeUsed = 0;
    }
}
