using System;
using System.Linq;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsProvider
    {
        int TotalFrames { get; }
        TimeSpan TotalTime { get; }
        TimeSpan FrameTime { get; }
        double Fps { get; }
        double AvgFps { get; }
    }

    internal sealed class PerformanceStatisticsProvider : IPerformanceStatisticsProvider
    {
        private readonly IPerformanceStatisticsStorage _performanceStatisticsStorage;

        public PerformanceStatisticsProvider(IPerformanceStatisticsStorage performanceStatisticsStorage)
        {
            _performanceStatisticsStorage = performanceStatisticsStorage;
        }

        public int TotalFrames => _performanceStatisticsStorage.TotalFrames;
        public TimeSpan TotalTime => _performanceStatisticsStorage.TotalTime;
        public TimeSpan FrameTime => _performanceStatisticsStorage.Frames.Last().Time;
        public double Fps => 1000 / FrameTime.TotalMilliseconds;

        public double AvgFps
        {
            get
            {
                var framesCount = 0;
                var allFramesTime = TimeSpan.Zero;
                foreach (var frame in _performanceStatisticsStorage.Frames)
                {
                    framesCount++;
                    allFramesTime += frame.Time;
                }

                return framesCount / allFramesTime.TotalMilliseconds * 1000;
            }
        }
    }
}