using System;
using System.Diagnostics;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsRecorder
    {
        void RecordFrame();
        void RecordSystemExecution(IFixedTimeStepSystem system, Action action);
        void RecordSystemExecution(IVariableTimeStepSystem system, Action action);
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

        public void RecordSystemExecution(IFixedTimeStepSystem system, Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            _performanceStatisticsStorage.AddSystemFrameTime(system.Name, stopwatch.Elapsed);
        }

        public void RecordSystemExecution(IVariableTimeStepSystem system, Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            _performanceStatisticsStorage.AddSystemFrameTime(system.Name, stopwatch.Elapsed);
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