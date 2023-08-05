/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using miniblocks.API.Platform;

namespace miniblocks.API.Timing;

public class ThrottledFrameClock : FramedClock, IDisposable
{
    private double accumulatedSleepError;
    private nint waitableTimer;

    public double MaximumUpdateFrequency { get; set; } = 1000.0;

    public double TimeSlept { get; private set; }

    public bool Throttling { get; set; } = true;

    internal ThrottledFrameClock()
    {
        waitableTimer = nint.Zero;

        // TODO!: We need Windows-specific code on the API.
        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            createWaitableTimer();
    }

    public override void ProcessFrame()
    {
        Debug.Assert(MaximumUpdateFrequency >= 0);

        base.ProcessFrame();

        if (Throttling)
        {
            if (MaximumUpdateFrequency > 0 && MaximumUpdateFrequency < double.MaxValue)
                throttle();
            else
                TimeSlept = sleepAndUpdateCurrent(0);
        }
        else
            TimeSlept = 0;

        Debug.Assert(TimeSlept <= ElapsedFrameTime);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void throttle()
    {
        var excessFrameTime = 1000.0d / MaximumUpdateFrequency - ElapsedFrameTime;

        TimeSlept = sleepAndUpdateCurrent(Math.Max(0, excessFrameTime + accumulatedSleepError));

        accumulatedSleepError += excessFrameTime - TimeSlept;
        accumulatedSleepError = Math.Max(-1000 / 30.0, accumulatedSleepError);
    }

    private double sleepAndUpdateCurrent(double ms)
    {
        if (ms <= 0) return 0;

        var before = CurrentTime;
        var span = TimeSpan.FromMilliseconds(ms);

        if (!waitWaitableTimer(span))
            Thread.Sleep(span);

        return (CurrentTime = SourceTime) - before;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool waitWaitableTimer(TimeSpan span)
    {
        if (waitableTimer == nint.Zero) return false;

        // TODO!: We need Windows-specific code on the API.

        return false;
    }

    private void createWaitableTimer()
        => throw new NotImplementedException("Windows-specific code is not implemented yet.");
}
