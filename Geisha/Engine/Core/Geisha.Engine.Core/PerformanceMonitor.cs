using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    // TODO move to Diagnostics?
    public static class PerformanceMonitor
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static readonly List<TimeSpan> FrameTimes = new List<TimeSpan>();

        private static readonly Dictionary<Type, List<FrameTimeRecord>> VariableTimeStepSystemsFrameTimes =
            new Dictionary<Type, List<FrameTimeRecord>>();

        private static readonly Dictionary<Type, List<FrameTimeRecord>> FixedTimeStepSystemsFrameTimes =
            new Dictionary<Type, List<FrameTimeRecord>>();

        // Greater than one to not get into NaN as first frame has time 0
        private static bool AnyFrames => FrameTimes.Count > 1;

        public static long TotalFrames => FrameTimes.Count;
        public static double TotalTime => FrameTimes.Select(t => t.TotalMilliseconds).Sum();
        public static double FrameTime => AnyFrames ? FrameTimes[FrameTimes.Count - 1].TotalMilliseconds : 0;

        public static double SmoothedFrameTime
        {
            get
            {
                if (FrameTimes.Count < 11) return FrameTime;

                var lastEleven = FrameTimes.AsEnumerable().Reverse().Take(11).ToList();
                var filteredFromOutliers = lastEleven.OrderBy(t => t).Skip(2).Take(7).ToList();

                return filteredFromOutliers.Average(t => t.TotalMilliseconds);
            }
        }

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

        public static void Reset()
        {
            Stopwatch.Reset();
            FrameTimes.Clear();
            VariableTimeStepSystemsFrameTimes.Clear();
            FixedTimeStepSystemsFrameTimes.Clear();
        }

        public static void AddFrame()
        {
            FrameTimes.Add(Stopwatch.Elapsed);
            Stopwatch.Restart();
        }

        public static void RecordSystemExecution(IVariableTimeStepSystem system, Action action)
        {
            if (!VariableTimeStepSystemsFrameTimes.ContainsKey(system.GetType()))
                VariableTimeStepSystemsFrameTimes[system.GetType()] = new List<FrameTimeRecord>();

            var stopwatch = Stopwatch.StartNew();
            action();
            VariableTimeStepSystemsFrameTimes[system.GetType()].Add(new FrameTimeRecord
            {
                FrameNumber = TotalFrames,
                FrameTime = stopwatch.Elapsed
            });
        }

        public static void RecordSystemExecution(IFixedTimeStepSystem system, Action action)
        {
            if (!FixedTimeStepSystemsFrameTimes.ContainsKey(system.GetType()))
                FixedTimeStepSystemsFrameTimes[system.GetType()] = new List<FrameTimeRecord>();

            var stopwatch = Stopwatch.StartNew();
            action();
            FixedTimeStepSystemsFrameTimes[system.GetType()].Add(new FrameTimeRecord
            {
                FrameNumber = TotalFrames,
                FrameTime = stopwatch.Elapsed
            });
        }

        public static Dictionary<Type, int> GetTotalSystemsShare()
        {
            var variableFrames = VariableTimeStepSystemsFrameTimes.Select(pair => new
            {
                Type = pair.Key,
                TotalTime = pair.Value.Sum(record => record.FrameTime.TotalMilliseconds)
            });

            var fixedFrames = FixedTimeStepSystemsFrameTimes.Select(pair => new
            {
                Type = pair.Key,
                TotalTime = pair.Value.Sum(record => record.FrameTime.TotalMilliseconds)
            });

            return variableFrames.Concat(fixedFrames).GroupBy(pair => pair.Type).Select(group => new
            {
                Type = group.Key,
                TotalTime = group.Select(g => g.TotalTime).Sum()
            }).ToDictionary(pair => pair.Type, pair => (int) (pair.TotalTime * 100 / TotalTime));
        }

        private struct FrameTimeRecord
        {
            public long FrameNumber { get; set; }
            public TimeSpan FrameTime { get; set; }
        }
    }
}