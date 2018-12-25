using System;
using System.Collections.Generic;
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
        IEnumerable<Frame> Frames { get; }
        IReadOnlyDictionary<string, IEnumerable<Frame>> SystemsFrames { get; }

        void AddFrame(TimeSpan frameTime);
        void AddSystemFrameTime(string systemName, TimeSpan frameTime);
    }

    internal sealed class PerformanceStatisticsStorage : IPerformanceStatisticsStorage
    {
        private const int CircularBufferSize = 100;
        private readonly CircularBuffer<Frame> _frames = new CircularBuffer<Frame>(CircularBufferSize);
        private readonly IReadOnlyDictionary<string, CircularBuffer<Frame>> _systemsFrames;

        public PerformanceStatisticsStorage(ISystemsProvider systemsProvider)
        {
            var systemsFramesField = new Dictionary<string, CircularBuffer<Frame>>();
            _systemsFrames = systemsFramesField;

            var systemsFramesProperty = new Dictionary<string, IEnumerable<Frame>>();
            SystemsFrames = systemsFramesProperty;

            foreach (var fixedTimeStepSystem in systemsProvider.GetFixedTimeStepSystems())
            {
                var circularBuffer = new CircularBuffer<Frame>(CircularBufferSize);
                systemsFramesField.Add(fixedTimeStepSystem.Name, circularBuffer);
                systemsFramesProperty.Add(fixedTimeStepSystem.Name, circularBuffer);
            }

            foreach (var variableTimeStepSystem in systemsProvider.GetVariableTimeStepSystems())
            {
                var circularBuffer = new CircularBuffer<Frame>(CircularBufferSize);
                systemsFramesField.Add(variableTimeStepSystem.Name, circularBuffer);
                systemsFramesProperty.Add(variableTimeStepSystem.Name, circularBuffer);
            }
        }

        public int TotalFrames { get; private set; }
        public IEnumerable<Frame> Frames => _frames;
        public IReadOnlyDictionary<string, IEnumerable<Frame>> SystemsFrames { get; }

        public void AddFrame(TimeSpan frameTime)
        {
            TotalFrames++;
            _frames.Add(new Frame(TotalFrames, frameTime));
        }

        public void AddSystemFrameTime(string systemName, TimeSpan frameTime)
        {
            throw new NotImplementedException();
        }
    }
}