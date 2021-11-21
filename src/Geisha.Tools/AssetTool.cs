using System;
using System.IO;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

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

        public static string CreateTextureAsset(string sourceTextureFilePath)
        {
            var textureFileExtension = Path.GetExtension(sourceTextureFilePath);
            if (!IsSupportedTextureFileFormat(textureFileExtension))
            {
                throw new ArgumentException($"Texture file format {textureFileExtension} is not supported.", nameof(sourceTextureFilePath));
            }

            var directoryPath = Path.GetDirectoryName(sourceTextureFilePath) ??
                                throw new ArgumentException("The path does not point to any file.", nameof(sourceTextureFilePath));

            var textureAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(sourceTextureFilePath));

            var textureAssetFilePath = Path.Combine(directoryPath, textureAssetFileName);

            var textureAssetContent = new TextureAssetContent
            {
                TextureFilePath = Path.GetFileName(sourceTextureFilePath)
            };

            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Texture, textureAssetContent);
            assetData.Save(textureAssetFilePath);

            return textureAssetFilePath;
        }
    }
}