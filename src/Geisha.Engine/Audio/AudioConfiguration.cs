namespace Geisha.Engine.Audio
{
    /// <summary>
    ///     Configuration of engine audio subsystem.
    /// </summary>
    public sealed record AudioConfiguration
    {
        /// <summary>
        ///     If true, the sound output is enabled. Otherwise the sound output is disabled.
        /// </summary>
        public bool EnableSound { get; init; } = true;
    }
}