using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.Diagnostics
{
    internal class SystemExecutionTime
    {
        public SystemExecutionTime(string systemName, TimeSpan avgFrameTime, double avgFrameTimeShare)
        {
            SystemName = systemName;
            AvgFrameTime = avgFrameTime;
            AvgFrameTimeShare = avgFrameTimeShare;
        }

        public string SystemName { get; }
        public TimeSpan AvgFrameTime { get; }
        public double AvgFrameTimeShare { get; }
    }

    internal interface IPerformanceStatisticsProvider
    {
        int TotalFrames { get; }
        TimeSpan TotalTime { get; }
        TimeSpan FrameTime { get; }
        double Fps { get; }
        double AvgFps { get; }

        IEnumerable<SystemExecutionTime> GetSystemsExecutionTime();
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

        public IEnumerable<SystemExecutionTime> GetSystemsExecutionTime()
        {
            if (!_performanceStatisticsStorage.Frames.Any())
            {
                return Enumerable.Empty<SystemExecutionTime>();
            }

            var avgFrameTimeInSeconds = _performanceStatisticsStorage.Frames.Average(f => f.Time.TotalSeconds);

            return _performanceStatisticsStorage.SystemsFrames
                .Select(systemFrames =>
                {
                    var systemAvgFrameTimeInSeconds = systemFrames.Value.Average(f => f.Time.TotalSeconds);
                    var systemAvgFrameTime = TimeSpan.FromSeconds(systemAvgFrameTimeInSeconds);

                    return new SystemExecutionTime(
                        systemFrames.Key,
                        systemAvgFrameTime,
                        systemAvgFrameTimeInSeconds / avgFrameTimeInSeconds);
                });
        }
    }
}