using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common;

namespace Geisha.Engine.Core.Diagnostics
{
    internal class GameLoopStepStatistics
    {
        public GameLoopStepStatistics(string stepName, TimeSpan avgFrameTime, double avgFrameTimeShare)
        {
            StepName = stepName;
            AvgFrameTime = avgFrameTime;
            AvgFrameTimeShare = avgFrameTimeShare;
        }

        public string StepName { get; }
        public TimeSpan AvgFrameTime { get; }
        public double AvgFrameTimeShare { get; }
    }

    internal interface IPerformanceStatisticsProvider
    {
        int TotalFrames { get; }
        TimeSpan TotalTime { get; }
        TimeSpan FrameTime { get; }
        TimeSpan AvgFrameTime { get; }
        double Fps { get; }
        double AvgFps { get; }

        IEnumerable<GameLoopStepStatistics> GetGameLoopStatistics();
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
        public TimeSpan AvgFrameTime => HighResolutionTimeSpan.FromSeconds(_performanceStatisticsStorage.Frames.Average(f => f.Time.TotalSeconds));
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

        public IEnumerable<GameLoopStepStatistics> GetGameLoopStatistics()
        {
            if (!_performanceStatisticsStorage.Frames.Any())
            {
                return Enumerable.Empty<GameLoopStepStatistics>();
            }

            var avgFrameTimeInSeconds = _performanceStatisticsStorage.Frames.Average(f => f.Time.TotalSeconds);

            return _performanceStatisticsStorage.StepsFrames
                .Select(stepFrames =>
                {
                    var stepAvgFrameTimeInSeconds = stepFrames.Value.Average(f => f.Time.TotalSeconds);
                    var stepAvgFrameTime = HighResolutionTimeSpan.FromSeconds(stepAvgFrameTimeInSeconds);

                    return new GameLoopStepStatistics(
                        stepFrames.Key,
                        stepAvgFrameTime,
                        stepAvgFrameTimeInSeconds / avgFrameTimeInSeconds);
                });
        }
    }
}