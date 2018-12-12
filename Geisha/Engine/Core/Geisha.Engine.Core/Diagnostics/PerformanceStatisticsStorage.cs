using System;

namespace Geisha.Engine.Core.Diagnostics
{
    public interface IPerformanceStatisticsStorage
    {
        void AddFrame(TimeSpan frameTime);
        void AddSystemFrameTime(string systemName, TimeSpan frameTime);
    }

    public class PerformanceStatisticsStorage
    {
    }
}