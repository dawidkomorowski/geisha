using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    public sealed class Coroutine
    {
        private readonly IEnumerator<CoroutineInstruction> _coroutine;

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
            return new CoroutineInstruction();
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

        internal void Execute()
        {
            if (State == CoroutineState.Running)
            {
                _coroutine.MoveNext();
            }
        }
    }

    public sealed class CoroutineInstruction
    {
    }

    public enum CoroutineState
    {
        Pending,
        Running,
        Paused,
        Stopped
    }
}