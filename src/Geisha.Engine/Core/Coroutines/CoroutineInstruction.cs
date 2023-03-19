namespace Geisha.Engine.Core.Coroutines
{
    /// <summary>
    ///     Represents instruction controlling execution of <see cref="Coroutine" />.
    /// </summary>
    /// <remarks>
    ///     Enumerator providing the coroutine implementation must use <see cref="CoroutineInstruction" /> as type
    ///     parameter. Use <see cref="Coroutine"/> class static methods to create specific instructions.
    /// </remarks>
    /// <seealso cref="Coroutine.Call"/>
    /// <seealso cref="Coroutine.SwitchTo"/>
    /// <seealso cref="Coroutine.WaitForNextFrame"/>
    /// <seealso cref="Coroutine.Wait"/>
    /// <seealso cref="Coroutine.WaitUntil"/>
    public abstract class CoroutineInstruction
    {
        internal abstract bool IsCompleted(GameTime gameTime);
        internal abstract void Execute(Coroutine coroutine);
    }
}