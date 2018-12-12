using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsProvider
    {
        int TotalFrames { get; }
    }

    internal sealed class PerformanceStatisticsRecorderAndProvider : IPerformanceStatisticsProvider, IPerformanceStatisticsRecorder
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static readonly List<TimeSpan> FrameTimes = new List<TimeSpan>();

        private static readonly Dictionary<string, List<FrameTimeRecord>> VariableTimeStepSystemsFrameTimes =
            new Dictionary<string, List<FrameTimeRecord>>();

        private static readonly Dictionary<string, List<FrameTimeRecord>> FixedTimeStepSystemsFrameTimes =
            new Dictionary<string, List<FrameTimeRecord>>();

        public PerformanceStatisticsRecorderAndProvider()
        {
        }

        // Greater than one to not get into NaN as first frame has time 0
        private static bool AnyFrames => FrameTimes.Count > 1;

        public int TotalFrames => FrameTimes.Count;
        public static double TotalTime => FrameTimes.Select(t => t.TotalMilliseconds).Sum();
        public static double FrameTime => AnyFrames ? FrameTimes[FrameTimes.Count - 1].TotalMilliseconds : 0;

        public static double Fps => AnyFrames ? 1000 / FrameTime : 0;

        public static double RealFps
        {
            get
            {
                var totalTime = TimeSpan.Zero;
                var realFps = FrameTimes.AsEnumerable().Reverse().TakeWhile(t =>
                {
                    totalTime += t;
                    return totalTime.TotalSeconds < 1;
                }).Count();
                return totalTime.TotalSeconds < 1 ? Fps : realFps;
            }
        }

        public void RecordFrame()
        {
            FrameTimes.Add(Stopwatch.Elapsed);
            Stopwatch.Restart();
        }

        public void RecordSystemExecution(IVariableTimeStepSystem system, Action action)
        {
            if (!VariableTimeStepSystemsFrameTimes.ContainsKey(system.Name))
                VariableTimeStepSystemsFrameTimes[system.Name] = new List<FrameTimeRecord>();

            var stopwatch = Stopwatch.StartNew();
            action();
            VariableTimeStepSystemsFrameTimes[system.Name].Add(new FrameTimeRecord
            {
                FrameNumber = TotalFrames,
                FrameTime = stopwatch.Elapsed
            });
        }

        public void RecordSystemExecution(IFixedTimeStepSystem system, Action action)
        {
            if (!FixedTimeStepSystemsFrameTimes.ContainsKey(system.Name))
                FixedTimeStepSystemsFrameTimes[system.Name] = new List<FrameTimeRecord>();

            var stopwatch = Stopwatch.StartNew();
            action();
            FixedTimeStepSystemsFrameTimes[system.Name].Add(new FrameTimeRecord
            {
                FrameNumber = TotalFrames,
                FrameTime = stopwatch.Elapsed
            });
        }

        public static Dictionary<string, int> GetTotalSystemsShare()
        {
            var variableFrames = VariableTimeStepSystemsFrameTimes.Select(pair => new
            {
                Name = pair.Key,
                TotalTime = pair.Value.AsEnumerable().Sum(record => record.FrameTime.TotalMilliseconds)
            });

            var fixedFrames = FixedTimeStepSystemsFrameTimes.Select(pair => new
            {
                Name = pair.Key,
                TotalTime = pair.Value.Sum(record => record.FrameTime.TotalMilliseconds)
            });

            return variableFrames.Concat(fixedFrames).GroupBy(pair => pair.Name).Select(group => new
            {
                Name = group.Key,
                TotalTime = group.Select(g => g.TotalTime).Sum()
            }).ToDictionary(pair => pair.Name, pair => (int) (pair.TotalTime * 100 / TotalTime));
        }

        public Dictionary<string, int> GetNLastFramesSystemsShare(int nLastFrames)
        {
            var startFrameIndex = TotalFrames - nLastFrames;
            var endFrameIndex = TotalFrames;

            var variableFrames = VariableTimeStepSystemsFrameTimes.Select(pair => new
            {
                Name = pair.Key,
                TotalTime = pair.Value.Where(record => record.FrameNumber > startFrameIndex && record.FrameNumber <= endFrameIndex)
                    .Sum(record => record.FrameTime.TotalMilliseconds)
            });

            var fixedFrames = FixedTimeStepSystemsFrameTimes.Select(pair => new
            {
                Name = pair.Key,
                TotalTime = pair.Value.Where(record => record.FrameNumber > startFrameIndex && record.FrameNumber <= endFrameIndex)
                    .Sum(record => record.FrameTime.TotalMilliseconds)
            });

            var nLastFramesTotalTime = FrameTimes.AsEnumerable().Reverse().Take(nLastFrames).Sum(ts => ts.TotalMilliseconds);
            return variableFrames.Concat(fixedFrames).GroupBy(pair => pair.Name).Select(group => new
            {
                Name = group.Key,
                TotalTime = group.Select(g => g.TotalTime).Sum()
            }).ToDictionary(pair => pair.Name, pair => (int) (pair.TotalTime * 100 / nLastFramesTotalTime));
        }

        private struct FrameTimeRecord
        {
            public long FrameNumber { get; set; }
            public TimeSpan FrameTime { get; set; }
        }
    }
}