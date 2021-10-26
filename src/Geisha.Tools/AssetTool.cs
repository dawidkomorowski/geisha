using System;
using Geisha.Engine.Audio;

namespace Geisha.Tools
{
    public static class AssetTool
    {
        public static bool IsSupportedSoundFileFormat(string fileExtension)
        {
            return SoundFormatParser.IsSupportedFileExtension(fileExtension);
        }

        public static string CreateSoundAsset(string sourceSoundFilePath)
        {
            throw new NotImplementedException();
        }
    }
}