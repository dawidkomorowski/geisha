using System;
using System.IO;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;

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
            var soundFileExtension = Path.GetExtension(sourceSoundFilePath);
            if (!IsSupportedSoundFileFormat(soundFileExtension))
            {
                throw new ArgumentException($"Sound file format {soundFileExtension} is not supported.", nameof(sourceSoundFilePath));
            }

            var directoryPath = Path.GetDirectoryName(sourceSoundFilePath) ??
                                throw new ArgumentException("The path does not point to any file.", nameof(sourceSoundFilePath));

            var soundAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(sourceSoundFilePath));

            var soundAssetFilePath = Path.Combine(directoryPath, soundAssetFileName);

            var soundAssetContent = new SoundAssetContent
            {
                SoundFilePath = Path.GetFileName(sourceSoundFilePath)
            };

            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), AudioAssetTypes.Sound, soundAssetContent);
            assetData.Save(soundAssetFilePath);

            return soundAssetFilePath;
        }

        public static bool IsSupportedTextureFileFormat(string fileExtension)
        {
            return fileExtension switch
            {
                ".bmp" => true,
                ".png" => true,
                ".jpg" => true,
                _ => false
            };
        }
    }
}