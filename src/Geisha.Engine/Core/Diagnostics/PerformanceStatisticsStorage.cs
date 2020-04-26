using System;
using System.Collections.Generic;
using Geisha.Common;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Diagnostics
{
    internal readonly struct Frame
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
        IReadOnlyCollection<Frame> Frames { get; }
        IReadOnlyDictionary<string, IReadOnlyCollection<Frame>> SystemsFrames { get; }

        void AddFrame(TimeSpan frameTime);
        void AddSystemFrameTime(string systemName, TimeSpan frameTime);
    }

    internal sealed class PerformanceStatisticsStorage : IPerformanceStatisticsStorage
    {
        private const int CircularBufferSize = 100;
        private readonly CircularBuffer<Frame> _frames = new CircularBuffer<Frame>(CircularBufferSize);
        private readonly Dictionary<string, CircularBuffer<Frame>> _systemsFrames;
        private readonly Dictionary<string, TimeSpan> _currentSystemsFrameTimes;
        private readonly IReadOnlyCollection<string> _systemsNames;

        public PerformanceStatisticsStorage(IEngineSystems engineSystems)
        {
            var systemsFramesField = new Dictionary<string, CircularBuffer<Frame>>();
            _systemsFrames = systemsFramesField;

            var systemsFramesProperty = new Dictionary<string, IReadOnlyCollection<Frame>>();
            SystemsFrames = systemsFramesProperty;

            var currentSystemsFrames = new Dictionary<string, TimeSpan>();
            _currentSystemsFrameTimes = currentSystemsFrames;

            _systemsNames = engineSystems.SystemsNames;
            foreach (var systemName in _systemsNames)
            {
                var circularBuffer = new CircularBuffer<Frame>(CircularBufferSize);
                systemsFramesField.Add(systemName, circularBuffer);
                systemsFramesProperty.Add(systemName, circularBuffer);
                currentSystemsFrames.Add(systemName, TimeSpan.Zero);
            }
        }

        public int TotalFrames { get; private set; }
        public TimeSpan TotalTime { get; private set; }
        public IReadOnlyCollection<Frame> Frames => _frames;
        public IReadOnlyDictionary<string, IReadOnlyCollection<Frame>> SystemsFrames { get; }

        public void AddFrame(TimeSpan frameTime)
        {
            TotalFrames++;
            TotalTime += frameTime;

            _frames.Add(new Frame(TotalFrames, frameTime));

            foreach (var systemFrameTime in _currentSystemsFrameTimes)
            {
                _systemsFrames[systemFrameTime.Key].Add(new Frame(TotalFrames, systemFrameTime.Value));
            }

            foreach (var systemName in _systemsNames)
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