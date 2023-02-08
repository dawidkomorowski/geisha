using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    public sealed class Coroutine
    {
        private readonly IEnumerator<CoroutineInstruction> _coroutine;
        private CoroutineInstruction _instruction = new WaitForNextFrameCoroutineInstruction();

        private Coroutine(IEnumerator<CoroutineInstruction> coroutine)
        {
            _coroutine = coroutine;
        }

        public static Coroutine Create(IEnumerator<CoroutineInstruction> coroutine)
        {
            return new Coroutine(coroutine);
        }

        public static CoroutineInstruction WaitForNextFrame()
        {
            return new WaitForNextFrameCoroutineInstruction();
        }

        public static CoroutineInstruction Wait(TimeSpan waitTime)
        {
            return new WaitCoroutineInstruction(waitTime);
        }

        public CoroutineState State { get; private set; } = CoroutineState.Pending;

        public void Pause()
        {
            State = CoroutineState.Paused;
        }

        public void Resume()
        {
            State = CoroutineState.Running;
        }

        public void Stop()
        {
            State = CoroutineState.Stopped;
        }

        internal void OnStarted()
        {
            State = CoroutineState.Running;
        }

        internal void Execute(GameTime gameTime)
        {
            if (State == CoroutineState.Running)
            {
                if (_instruction.ShouldExecute(gameTime))
                {
                    _coroutine.MoveNext();
                    _instruction = _coroutine.Current;
                }
            }
        }
    }

    public abstract class CoroutineInstruction
    {
        internal abstract bool ShouldExecute(GameTime gameTime);
    }

    internal sealed class WaitForNextFrameCoroutineInstruction : CoroutineInstruction
    {
        internal override bool ShouldExecute(GameTime gameTime) => true;
    }

    internal sealed class WaitCoroutineInstruction : CoroutineInstruction
    {
        private readonly TimeSpan _waitTime;
        private TimeSpan _timeWaited = TimeSpan.Zero;

        public WaitCoroutineInstruction(TimeSpan waitTime)
        {
            _waitTime = waitTime;
        }

        internal override bool ShouldExecute(GameTime gameTime)
        {
            _timeWaited += gameTime.DeltaTime;
            return _timeWaited >= _waitTime;
        }
    }

    public enum CoroutineState
    {
        Pending,
        Running,
        Paused,
        Stopped
    }
}