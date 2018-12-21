using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Diagnostics
{
    public interface IPerformanceStatisticsStorage
    {
        int TotalFrames { get; }
        IReadOnlyList<TimeSpan> Frames { get; }

        void AddFrame(TimeSpan frameTime);
        void AddSystemFrameTime(string systemName, TimeSpan frameTime);
    }

    public sealed class PerformanceStatisticsStorage : IPerformanceStatisticsStorage
    {
        public int TotalFrames { get; }
        public IReadOnlyList<TimeSpan> Frames { get; } = new List<TimeSpan>(100);

        public void AddFrame(TimeSpan frameTime)
        {
            throw new NotImplementedException();
        }

        public void AddSystemFrameTime(string systemName, TimeSpan frameTime)
        {
            throw new NotImplementedException();
        }
    }
}