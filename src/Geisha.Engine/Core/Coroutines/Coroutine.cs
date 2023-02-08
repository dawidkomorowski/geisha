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

        public static CoroutineInstruction WaitUntil(Func<bool> condition)
        {
            return new WaitUntilCoroutineInstruction(condition);
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

        public void Abort()
        {
            State = CoroutineState.Aborted;
        }

        internal void OnStart()
        {
            State = CoroutineState.Running;
        }

        internal void Execute(GameTime gameTime)
        {
            if (State == CoroutineState.Running)
            {
                if (_instruction.ShouldExecute(gameTime))
                {
                    if (!_coroutine.MoveNext())
                    {
                        State = CoroutineState.Completed;
                        return;
                    }

                    _instruction = _coroutine.Current;
                }
            }
            else if (State is CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be executed.");
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

    internal sealed class WaitUntilCoroutineInstruction : CoroutineInstruction
    {
        private readonly Func<bool> _condition;

        public WaitUntilCoroutineInstruction(Func<bool> condition)
        {
            _condition = condition;
        }

        internal override bool ShouldExecute(GameTime gameTime) => _condition();
    }

    public enum CoroutineState
    {
        Pending,
        Running,
        Paused,
        Completed,
        Aborted
    }
}