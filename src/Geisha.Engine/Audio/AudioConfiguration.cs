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

        /// <summary>
        ///     Master volume of sound output. Valid range is from <c>0.0</c> meaning no audio, to <c>1.0</c> meaning maximum audio
        ///     volume.
        /// </summary>
        public double Volume { get; init; } = 1.0;
    }
}