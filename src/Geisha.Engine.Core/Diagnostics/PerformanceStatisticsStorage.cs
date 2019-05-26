using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Diagnostics
{
    internal struct Frame
    {
        public Frame(int number, TimeSpan time)
        {
            Number = number;
            Time = time;
        }

        public int Number { get; }
        public TimeSpan Time { get; }
    }

    internal interface IPerformanceStatisticsStorage
    {
        int TotalFrames { get; }
        TimeSpan TotalTime { get; }
        IEnumerable<Frame> Frames { get; }
        IReadOnlyDictionary<string, IEnumerable<Frame>> SystemsFrames { get; }

        void AddFrame(TimeSpan frameTime);
        void AddSystemFrameTime(string systemName, TimeSpan frameTime);
    }

    internal sealed class PerformanceStatisticsStorage : IPerformanceStatisticsStorage
    {
        private const int CircularBufferSize = 100;
        private readonly CircularBuffer<Frame> _frames = new CircularBuffer<Frame>(CircularBufferSize);
        private readonly Dictionary<string, CircularBuffer<Frame>> _systemsFrames;
        private readonly Dictionary<string, TimeSpan> _currentSystemsFrameTimes;
        private readonly List<string> _systemNames;

        public PerformanceStatisticsStorage(ISystemsProvider systemsProvider)
        {
            var systemsFramesField = new Dictionary<string, CircularBuffer<Frame>>();
            _systemsFrames = systemsFramesField;

            var systemsFramesProperty = new Dictionary<string, IEnumerable<Frame>>();
            SystemsFrames = systemsFramesProperty;

            var currentSystemsFrames = new Dictionary<string, TimeSpan>();
            _currentSystemsFrameTimes = currentSystemsFrames;

            var fixedTimeStepSystemsNames = systemsProvider.GetFixedTimeStepSystems().Select(s => s.Name);
            var variableTimesStepSystemsNames = systemsProvider.GetVariableTimeStepSystems().Select(s => s.Name);
            _systemNames = fixedTimeStepSystemsNames.Concat(variableTimesStepSystemsNames).Distinct().ToList();

            foreach (var systemName in _systemNames)
            {
                var circularBuffer = new CircularBuffer<Frame>(CircularBufferSize);
                systemsFramesField.Add(systemName, circularBuffer);
                systemsFramesProperty.Add(systemName, circularBuffer);
                currentSystemsFrames.Add(systemName, TimeSpan.Zero);
            }
        }

        public int TotalFrames { get; private set; }
        public TimeSpan TotalTime { get; private set; }
        public IEnumerable<Frame> Frames => _frames;
        public IReadOnlyDictionary<string, IEnumerable<Frame>> SystemsFrames { get; }

        public void AddFrame(TimeSpan frameTime)
        {
            TotalFrames++;
            TotalTime += frameTime;

            _frames.Add(new Frame(TotalFrames, frameTime));

            foreach (var systemFrameTime in _currentSystemsFrameTimes)
            {
                _systemsFrames[systemFrameTime.Key].Add(new Frame(TotalFrames, systemFrameTime.Value));
            }

            foreach (var systemName in _systemNames)
            {
                _currentSystemsFrameTimes[systemName] = TimeSpan.Zero;
            }
        }

        public void AddSystemFrameTime(string systemName, TimeSpan frameTime)
        {
            _currentSystemsFrameTimes[systemName] += frameTime;
        }
    }
}