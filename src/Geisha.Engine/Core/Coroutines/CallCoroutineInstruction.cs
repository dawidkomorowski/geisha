﻿using System.Collections.Generic;

namespace Geisha.Engine.Core.Coroutines
{
    internal sealed class CallCoroutineInstruction : CoroutineInstruction
    {
        public CallCoroutineInstruction(IEnumerator<CoroutineInstruction> coroutine)
        {
            Coroutine = coroutine;
        }

        public IEnumerator<CoroutineInstruction> Coroutine { get; }

        internal override bool IsCompleted(GameTime gameTime) => true;

        internal override void Execute(Coroutine coroutine)
        {
            coroutine.HandleCallInstruction(this);
        }
    }
}