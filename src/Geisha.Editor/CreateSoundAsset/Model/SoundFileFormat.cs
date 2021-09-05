using Geisha.Engine.Audio;

namespace Geisha.Editor.CreateSoundAsset.Model
{
    internal static class SoundFileFormat
    {
        public static bool IsSupported(string fileExtension)
        {
            try
            {
                SoundFormatParser.ParseFromFileExtension(fileExtension);
                return true;
            }
            catch (UnsupportedSoundFileFormatException)
            {
                return false;
            }
        }
    }
}