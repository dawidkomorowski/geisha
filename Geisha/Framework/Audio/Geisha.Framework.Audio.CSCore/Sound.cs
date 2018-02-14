namespace Geisha.Framework.Audio.CSCore
{
    internal class Sound : ISound
    {
        public Sound(SharedMemoryStream soundStream, SoundFormat format)
        {
            SoundStream = soundStream;
            Format = format;
        }

        public SharedMemoryStream SoundStream { get; }
        public SoundFormat Format { get; }
    }
}