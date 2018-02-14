using System.IO;

namespace Geisha.Framework.Audio
{
    public interface IAudioProvider
    {
        ISound CreateSound(Stream stream, SoundFormat soundFormat);
        void Play(ISound sound);
    }
}