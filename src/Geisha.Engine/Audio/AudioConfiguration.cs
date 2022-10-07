namespace Geisha.Engine.Audio
{
    /// <summary>
    ///     Configuration of engine audio subsystem.
    /// </summary>
    public sealed class AudioConfiguration
    {
        private AudioConfiguration(bool enableSound)
        {
            EnableSound = enableSound;
        }


        /// <summary>
        ///     If true, the sound output is enabled. Otherwise the sound output is disabled.
        /// </summary>
        public bool EnableSound { get; }

        public static IBuilder CreateBuilder() => new Builder();

        public interface IBuilder
        {
            IBuilder WithEnableSound(bool enableSound);
            AudioConfiguration Build();
        }

        private sealed class Builder : IBuilder
        {
            private bool _enableSound = true;

            public IBuilder WithEnableSound(bool enableSound)
            {
                _enableSound = enableSound;
                return this;
            }

            public AudioConfiguration Build() => new(_enableSound);
        }
    }
}