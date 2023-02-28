using System;

namespace Geisha.Engine.Core.Coroutines
{
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
}