namespace Geisha.Engine.Core.Coroutines
{
    /// <summary>
    ///     Specifies update mode of <see cref="Coroutine" />.
    /// </summary>
    public enum CoroutineUpdateMode
    {
        /// <summary>
        ///     <see cref="Coroutine" /> is executed as part of fixed time step update in game loop.
        /// </summary>
        FixedTimeStep,

        /// <summary>
        ///     <see cref="Coroutine" /> is executed as part of variable time step update in game loop.
        /// </summary>
        VariableTimeStep
    }
}