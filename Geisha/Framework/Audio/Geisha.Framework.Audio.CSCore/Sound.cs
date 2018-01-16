namespace Geisha.Framework.Audio.CSCore
{
    internal class Sound : ISound
    {
        public Sound(SharedMemoryStream soundStream)
        {
            SoundStream = soundStream;
        }

        public SharedMemoryStream SoundStream { get; }
    }
}