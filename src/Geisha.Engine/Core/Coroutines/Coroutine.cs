using System;
using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    public sealed class Coroutine
    {
        private readonly Stack<IEnumerator<CoroutineInstruction>> _callStack = new();
        private CoroutineInstruction _instruction = new WaitForNextFrameCoroutineInstruction();

        private Coroutine(IEnumerator<CoroutineInstruction> coroutine)
        {
            _callStack.Push(coroutine);
        }

        public static Coroutine Create(IEnumerator<CoroutineInstruction> coroutine)
        {
            return new Coroutine(coroutine);
        }

        public static CoroutineInstruction Call(IEnumerator<CoroutineInstruction> coroutine)
        {
            return new CallCoroutineInstruction(coroutine);
        }

        public static CoroutineInstruction SwitchTo(Coroutine coroutine)
        {
            return new SwitchToCoroutineInstruction(coroutine);
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
            if (State is CoroutineState.Pending or CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be paused.");
            }

            State = CoroutineState.Paused;
        }

        public void Resume()
        {
            if (State is CoroutineState.Pending or CoroutineState.Completed or CoroutineState.Aborted)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be resumed.");
            }

            State = CoroutineState.Running;
        }

        public void Abort()
        {
            if (State is CoroutineState.Completed)
            {
                throw new InvalidOperationException($"Coroutine in state '{State}' cannot be aborted.");
            }

            State = CoroutineState.Aborted;
        }

        internal void OnStart()
        {
            State = CoroutineState.Running;
        }

        internal Coroutine? Execute(GameTime gameTime)
        {
            if (State == CoroutineState.Running)
            {
                if (_instruction.ShouldExecute(gameTime))
                {
                    var coroutine = _callStack.Peek();

                    while (!coroutine.MoveNext())
                    {
                        _callStack.Pop();

                        if (_callStack.Count == 0)
                        {
                            State = CoroutineState.Completed;
                            return null;
                        }

                        coroutine = _callStack.Peek();
                    }

                    _instruction = coroutine.Current;

                    switch (_instruction)
                    {
                        case CallCoroutineInstruction callInstruction:
                            _callStack.Push(callInstruction.Coroutine);
                            break;
                        case SwitchToCoroutineInstruction switchToInstruction:
                            return switchToInstruction.Coroutine;
                    }
                }

                return null;
            }

            return null;
        }
    }

    public abstract class CoroutineInstruction
    {
        internal abstract bool ShouldExecute(GameTime gameTime);
    }

    internal sealed class CallCoroutineInstruction : CoroutineInstruction
    {
        public CallCoroutineInstruction(IEnumerator<CoroutineInstruction> coroutine)
        {
            Coroutine = coroutine;
        }

        public IEnumerator<CoroutineInstruction> Coroutine { get; }

        internal override bool ShouldExecute(GameTime gameTime) => true;
    }

    internal sealed class SwitchToCoroutineInstruction : CoroutineInstruction
    {
        public SwitchToCoroutineInstruction(Coroutine coroutine)
        {
            Coroutine = coroutine;
        }

        public Coroutine Coroutine { get; }

        internal override bool ShouldExecute(GameTime gameTime)
        {
            return true;
        }
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