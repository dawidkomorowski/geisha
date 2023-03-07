namespace Geisha.Engine.Core.Coroutines
{
    internal sealed class WaitForNextFrameCoroutineInstruction : CoroutineInstruction
    {
        internal override bool IsCompleted(GameTime gameTime) => true;

        internal override void Execute(Coroutine coroutine)
        {
        }
    }
}