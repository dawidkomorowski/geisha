using System;

namespace Geisha.Engine.Core.Coroutines
{
    internal sealed class WaitUntilCoroutineInstruction : CoroutineInstruction
    {
        private readonly Func<bool> _condition;

        public WaitUntilCoroutineInstruction(Func<bool> condition)
        {
            _condition = condition;
        }

        internal override bool ShouldExecute(GameTime gameTime) => _condition();
    }
}