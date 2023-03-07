namespace Geisha.Engine.Core.Coroutines
{
    public abstract class CoroutineInstruction
    {
        internal abstract bool IsCompleted(GameTime gameTime);
        internal abstract void Execute(Coroutine coroutine);
    }
}