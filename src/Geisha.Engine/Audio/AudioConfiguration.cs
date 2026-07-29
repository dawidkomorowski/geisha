namespace Geisha.Engine.Audio
{
    /// <summary>
    ///     Configuration of engine audio subsystem.
    /// </summary>
    public sealed record AudioConfiguration
    {
        /// <summary>
        ///     Specifies whether the sound output is enabled. When disabled, no audio is played. Default is <c>true</c>.
        /// </summary>
        public bool EnableSound { get; init; } = true;

        /// <summary>
        ///     Master volume of sound output. Valid range is from <c>0.0</c> meaning no audio, to <c>1.0</c> meaning maximum audio
        ///     volume. Default is <c>1.0</c>.
        /// </summary>
        public double Volume { get; init; } = 1.0;
    }
}