namespace Geisha.Engine.Core.Coroutines
{
    internal sealed class WaitForNextFrameCoroutineInstruction : CoroutineInstruction
    {
        internal override bool ShouldExecute(GameTime gameTime) => true;
    }
}