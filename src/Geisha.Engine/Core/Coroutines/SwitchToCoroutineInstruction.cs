namespace Geisha.Engine.Core.Coroutines
{
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
}