using System.IO;

namespace Geisha.Framework.Audio
{
    public interface ISoundPlayer
    {
        ISound CreateSound(Stream stream);
        void Play(ISound sound);
    }
}