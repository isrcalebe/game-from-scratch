/*
 * Part of the MINIBLOCKS, under the MIT License.
 * See COPYING for license information.
 * SPDX-License-Identifier: MIT
 */

#nullable enable

using System;
using System.Linq;
using miniblocks.API.Extensions.TypeExtensions;

namespace miniblocks.API.Timing;

public class FramedClock : IFrameBasedClock, ISourceChangeableClock
{
    private const int calculation_interval = 10;

    private long totalFramesProcessed;

    private double timeUntilNextCalculation;
    private double timeSinceLastCalculation;
    private int framesSinceLastCalculation;

    private readonly bool processSource;

    private readonly double[] betweenFrameTimes = new double[128];

    public IClock Source { get; private set; }

    public FrameTimeData Data
        => new()
        {
            Elapsed = ElapsedFrameTime,
            Current = CurrentTime
        };

    public double FramesPerSecond { get; private set; }

    public double Jitter { get; private set; }

    public double ElapsedFrameTime => CurrentTime - LastFrameTime;

    public double Rate => Source.Rate;

    public virtual double CurrentTime { get; protected set; }

    protected virtual double LastFrameTime { get; set; }

    protected double SourceTime => Source.CurrentTime;

    public bool IsRunning => Source.IsRunning;

    public FramedClock(IClock? source = null, bool processSource = true)
    {
        this.processSource = processSource;
        Source = source ?? new StopwatchClock(true);

        ChangeSource(Source);
    }

    public void ChangeSource(IClock? source)
    {
        if (source == null) return;

        CurrentTime = LastFrameTime = source.CurrentTime;
        Source = source;
    }

    public virtual void ProcessFrame()
    {
        betweenFrameTimes[totalFramesProcessed % betweenFrameTimes.Length] = CurrentTime - LastFrameTime;
        totalFramesProcessed++;

        if (processSource && Source is IFrameBasedClock framedSource)
            framedSource.ProcessFrame();

        if (timeUntilNextCalculation <= 0)
        {
            timeUntilNextCalculation += calculation_interval;

            if (framesSinceLastCalculation == 0)
            {
                FramesPerSecond = 0;
                Jitter = 0;
            }
            else
            {
                FramesPerSecond = (int)Math.Ceiling(framesSinceLastCalculation * 1000.0f / timeSinceLastCalculation);

                var average = betweenFrameTimes.Average();
                var root = Math.Sqrt(betweenFrameTimes.Average(item => Math.Pow(item - average, 2)));
                Jitter = root;
            }

            timeSinceLastCalculation = framesSinceLastCalculation = 0;
        }

        framesSinceLastCalculation++;
        timeUntilNextCalculation -= ElapsedFrameTime;
        timeSinceLastCalculation += ElapsedFrameTime;

        LastFrameTime = CurrentTime;
        CurrentTime = SourceTime;
    }

    public override string ToString()
        => $@"{GetType().ReadableName()} ({Math.Truncate(CurrentTime)}ms, {FramesPerSecond} FPS)";
}
