namespace Geisha.Engine.Core.Coroutines
{
    /// <summary>
    ///     Specifies state of <see cref="Coroutine" />.
    /// </summary>
    public enum CoroutineState
    {
        /// <summary>
        ///     <see cref="Coroutine" /> is ready to be started.
        /// </summary>
        Pending,

        /// <summary>
        ///     <see cref="Coroutine" /> is running.
        /// </summary>
        Running,

        /// <summary>
        ///     <see cref="Coroutine" /> is paused.
        /// </summary>
        Paused,

        /// <summary>
        ///     <see cref="Coroutine" /> has completed.
        /// </summary>
        Completed,

        /// <summary>
        ///     <see cref="Coroutine" /> was aborted.
        /// </summary>
        Aborted
    }
}