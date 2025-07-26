using System;
using System.Diagnostics;

namespace Geisha.Engine.Core.Diagnostics
{
    internal interface IPerformanceStatisticsRecorder
    {
        void RecordFrame();
        void BeginStepDuration();
        void EndStepDuration(string stepName);
    }

    internal sealed class PerformanceStatisticsRecorder : IPerformanceStatisticsRecorder
    {
        private readonly IPerformanceStatisticsStorage _performanceStatisticsStorage;
        private readonly Stopwatch _frameStopwatch = new();
        private readonly Stopwatch _stepStopwatch = new();
        private bool _stepDurationRunning;

        public PerformanceStatisticsRecorder(IPerformanceStatisticsStorage performanceStatisticsStorage)
        {
            _performanceStatisticsStorage = performanceStatisticsStorage;
        }

        public void RecordFrame()
        {
            _performanceStatisticsStorage.AddFrame(_frameStopwatch.Elapsed);
            _frameStopwatch.Restart();
        }

        public void BeginStepDuration()
        {
            if (_stepDurationRunning)
            {
                throw new InvalidOperationException($"Step duration recording is already running. Call {nameof(EndStepDuration)} before starting a new one.");
            }

            _stepDurationRunning = true;
            _stepStopwatch.Restart();
        }

        public void EndStepDuration(string stepName)
        {
            if (!_stepDurationRunning)
            {
                throw new InvalidOperationException($"Step duration recording is not running. Call {nameof(BeginStepDuration)} before ending it.");
            }

            _stepDurationRunning = false;
            _performanceStatisticsStorage.AddStepFrameTime(stepName, _stepStopwatch.Elapsed);
        }
    }
}