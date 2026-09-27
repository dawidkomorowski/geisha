using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.GameLoop;

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
        IReadOnlyDictionary<string, IReadOnlyCollection<Frame>> StepsFrames { get; }

        void AddFrame(TimeSpan frameTime);
        void AddStepFrameTime(string stepName, TimeSpan frameTime);
    }

    internal sealed class PerformanceStatisticsStorage : IPerformanceStatisticsStorage
    {
        private const int CircularBufferSize = 100;
        private readonly CircularBuffer<Frame> _frames = new(CircularBufferSize);
        private readonly Dictionary<string, CircularBuffer<Frame>> _stepsFrames = new();
        private readonly Dictionary<string, TimeSpan> _currentStepsFrameTimes = new();
        private readonly IReadOnlyCollection<string> _stepsNames;

        public PerformanceStatisticsStorage(IGameLoopSteps gameLoopSteps)
        {
            var stepsFramesProperty = new Dictionary<string, IReadOnlyCollection<Frame>>();
            StepsFrames = stepsFramesProperty;

            _stepsNames = gameLoopSteps.StepsNames;
            foreach (var stepName in _stepsNames)
            {
                var circularBuffer = new CircularBuffer<Frame>(CircularBufferSize);
                _stepsFrames.Add(stepName, circularBuffer);
                stepsFramesProperty.Add(stepName, circularBuffer);
                _currentStepsFrameTimes.Add(stepName, TimeSpan.Zero);
            }
        }

        public int TotalFrames { get; private set; }
        public TimeSpan TotalTime { get; private set; }
        public IReadOnlyCollection<Frame> Frames => _frames;
        public IReadOnlyDictionary<string, IReadOnlyCollection<Frame>> StepsFrames { get; }

        public void AddFrame(TimeSpan frameTime)
        {
            TotalFrames++;
            TotalTime += frameTime;

            _frames.Add(new Frame(TotalFrames, frameTime));

            foreach (var (stepName, stepFrameTime) in _currentStepsFrameTimes)
            {
                _stepsFrames[stepName].Add(new Frame(TotalFrames, stepFrameTime));
            }

            foreach (var stepName in _stepsNames)
            {
                _currentStepsFrameTimes[stepName] = TimeSpan.Zero;
            }
        }

        public void AddStepFrameTime(string stepName, TimeSpan frameTime)
        {
            _currentStepsFrameTimes[stepName] += frameTime;
        }
    }
}