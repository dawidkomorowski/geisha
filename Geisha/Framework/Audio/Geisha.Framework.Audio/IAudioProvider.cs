using System.IO;

namespace Geisha.Framework.Audio
{
    public interface IAudioProvider
    {
        ISound CreateSound(Stream stream);
        void Play(ISound sound);
    }
}