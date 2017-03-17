using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    public static class PerformanceMonitor
    {
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static readonly List<TimeSpan> FrameTimes = new List<TimeSpan>();

        private static readonly Dictionary<Type, List<FrameTimeRecord>> VariableSystemsFrameTimes =
            new Dictionary<Type, List<FrameTimeRecord>>();

        private static readonly Dictionary<Type, List<FrameTimeRecord>> FixedSystemsFrameTimes =
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

        private struct FrameTimeRecord
        {
            public long FrameNumber { get; set; }
            public TimeSpan FrameTime { get; set; }
        }

        public static void Reset()
        {
            Stopwatch.Reset();
            FrameTimes.Clear();
            VariableSystemsFrameTimes.Clear();
            FixedSystemsFrameTimes.Clear();
        }

        public static void AddFrame()
        {
            FrameTimes.Add(Stopwatch.Elapsed);
            Stopwatch.Restart();
        }

        public static void RecordVariableSystemExecution(ISystem system, Action action)
        {
            if (!VariableSystemsFrameTimes.ContainsKey(system.GetType()))
            {
                VariableSystemsFrameTimes[system.GetType()] = new List<FrameTimeRecord>();
            }

            var stopwatch = Stopwatch.StartNew();
            action();
            VariableSystemsFrameTimes[system.GetType()].Add(new FrameTimeRecord
            {
                FrameNumber = TotalFrames,
                FrameTime = stopwatch.Elapsed
            });
        }

        public static void RecordFixedSystemExecution(ISystem system, Action action)
        {
            if (!FixedSystemsFrameTimes.ContainsKey(system.GetType()))
            {
                FixedSystemsFrameTimes[system.GetType()] = new List<FrameTimeRecord>();
            }

            var stopwatch = Stopwatch.StartNew();
            action();
            FixedSystemsFrameTimes[system.GetType()].Add(new FrameTimeRecord
            {
                FrameNumber = TotalFrames,
                FrameTime = stopwatch.Elapsed
            });
        }

        public static Dictionary<Type, int> GetTotalSystemsShare()
        {
            var variableFrames = VariableSystemsFrameTimes.Select(pair => new
            {
                Type = pair.Key,
                TotalTime = pair.Value.Sum(record => record.FrameTime.TotalMilliseconds)
            });

            var fixedFrames = FixedSystemsFrameTimes.Select(pair => new
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
    }
}