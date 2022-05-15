using System;
using System.Diagnostics;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsRecorder
    {
        void RecordFrame();
        IDisposable RecordStepDuration(string stepName);
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

        public IDisposable RecordStepDuration(string stepName)
        {
            return new RecordingScope(_performanceStatisticsStorage, stepName);
        }

        private sealed class RecordingScope : IDisposable
        {
            private readonly IPerformanceStatisticsStorage _performanceStatisticsStorage;
            private readonly string _stepName;
            private readonly Stopwatch _stopwatch;

            public RecordingScope(IPerformanceStatisticsStorage performanceStatisticsStorage, string stepName)
            {
                _performanceStatisticsStorage = performanceStatisticsStorage;
                _stepName = stepName;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _performanceStatisticsStorage.AddStepFrameTime(_stepName, _stopwatch.Elapsed);
            }
        }
    }
}