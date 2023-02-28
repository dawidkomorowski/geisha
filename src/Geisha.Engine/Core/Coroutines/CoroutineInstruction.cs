namespace Geisha.Engine.Core.Coroutines
{
    public abstract class CoroutineInstruction
    {
        internal abstract bool ShouldExecute(GameTime gameTime);
    }
}