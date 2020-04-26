using System;
using System.Diagnostics;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsRecorder
    {
        void RecordFrame();
        IDisposable RecordSystemExecution(string systemName);
    }

    internal sealed class PerformanceStatisticsRecorder : IPerformanceStatisticsRecorder
    {
        private readonly IPerformanceStatisticsStorage _performanceStatisticsStorage;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public PerformanceStatisticsRecorder(IPerformanceStatisticsStorage performanceStatisticsStorage)
        {
            _performanceStatisticsStorage = performanceStatisticsStorage;
        }

        public void RecordFrame()
        {
            _performanceStatisticsStorage.AddFrame(_stopwatch.Elapsed);
            _stopwatch.Restart();
        }

        public IDisposable RecordSystemExecution(string systemName)
        {
            return new RecordingScope(_performanceStatisticsStorage, systemName);
        }

        private sealed class RecordingScope : IDisposable
        {
            private readonly IPerformanceStatisticsStorage _performanceStatisticsStorage;
            private readonly string _systemName;
            private readonly Stopwatch _stopwatch;

            public RecordingScope(IPerformanceStatisticsStorage performanceStatisticsStorage, string systemName)
            {
                _performanceStatisticsStorage = performanceStatisticsStorage;
                _systemName = systemName;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _performanceStatisticsStorage.AddSystemFrameTime(_systemName, _stopwatch.Elapsed);
            }
        }
    }
}