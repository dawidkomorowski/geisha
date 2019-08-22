using Geisha.Engine.Audio;

namespace Geisha.Framework.Audio.CSCore
{
    internal sealed class Sound : ISound
    {
        public Sound(SharedMemoryStream soundStream, SoundFormat format)
        {
            SoundStream = soundStream;
            Format = format;
        }

        public SharedMemoryStream SoundStream { get; }
        public SoundFormat Format { get; }

        /// <summary>
        ///     Disposes <see cref="SoundStream" /> instance owned by the <see cref="Sound" />. Actual memory will be released when
        ///     all instances of the sound will complete playing and shared instances of <see cref="SharedMemoryStream" /> will be
        ///     disposed.
        /// </summary>
        public void Dispose()
        {
            SoundStream.Dispose();
        }
    }
}