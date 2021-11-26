using System;
using System.Drawing;
using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
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

            var soundAssetContent = new SoundAssetContent
            {
                SoundFilePath = Path.GetFileName(sourceSoundFilePath)
            };

            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), AudioAssetTypes.Sound, soundAssetContent);

            var soundAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(sourceSoundFilePath));
            var soundAssetFilePath = Path.Combine(directoryPath, soundAssetFileName);
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

            var textureAssetContent = new TextureAssetContent
            {
                TextureFilePath = Path.GetFileName(sourceTextureFilePath)
            };

            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Texture, textureAssetContent);

            var textureAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(sourceTextureFilePath));
            var textureAssetFilePath = Path.Combine(directoryPath, textureAssetFileName);
            assetData.Save(textureAssetFilePath);

            return textureAssetFilePath;
        }

        public static bool CanCreateSpriteAssetFromFile(string filePath)
        {
            return IsTextureAssetFile(filePath) || IsTextureFile(filePath);
        }

        public static (string spriteAssetFilePath, string? textureAssetFilePath) CreateSpriteAsset(string sourceTextureFilePath)
        {
            if (IsTextureFile(sourceTextureFilePath))
            {
                var textureAssetFilePath = CreateTextureAsset(sourceTextureFilePath);
                var spriteAssetFilePath = CreateSpriteFromTextureAssetFile(textureAssetFilePath);
                return (spriteAssetFilePath, textureAssetFilePath);
            }

            if (IsTextureAssetFile(sourceTextureFilePath))
            {
                var spriteAssetFilePath = CreateSpriteFromTextureAssetFile(sourceTextureFilePath);
                return (spriteAssetFilePath, null);
            }

            throw new ArgumentException(
                $"Given file is neither texture asset file ({AssetFileUtils.Extension}) nor texture file (i.e. .png).", nameof(sourceTextureFilePath));
        }

        private static bool IsTextureAssetFile(string filePath)
        {
            if (!AssetFileUtils.IsAssetFile(filePath)) return false;
            return AssetData.Load(filePath).AssetType == RenderingAssetTypes.Texture;
        }

        private static bool IsTextureFile(string filePath)
        {
            return IsSupportedTextureFileFormat(Path.GetExtension(filePath));
        }

        private static string CreateSpriteFromTextureAssetFile(string textureAssetFilePath)
        {
            var textureAssetData = AssetData.Load(textureAssetFilePath);
            var textureAssetContent = textureAssetData.ReadJsonContent<TextureAssetContent>();

            if (textureAssetContent.TextureFilePath == null)
                throw new ArgumentException($"{nameof(TextureAssetContent)}.{nameof(TextureAssetContent.TextureFilePath)} cannot be null.");

            var textureImageFilePath = PathUtils.GetSiblingPath(textureAssetFilePath, textureAssetContent.TextureFilePath);
            Vector2 spriteDimension;
            using (var bitmapImage = Image.FromFile(textureImageFilePath))
            {
                spriteDimension = new Vector2(bitmapImage.Width, bitmapImage.Height);
            }

            var directoryPath = Path.GetDirectoryName(textureAssetFilePath) ??
                                throw new ArgumentException("The path does not point to any file.", nameof(textureAssetFilePath));

            var spriteAssetContent = new SpriteAssetContent
            {
                TextureAssetId = textureAssetData.AssetId.Value,
                SourceUV = SerializableVector2.FromVector2(Vector2.Zero),
                SourceDimension = SerializableVector2.FromVector2(spriteDimension),
                SourceAnchor = SerializableVector2.FromVector2(spriteDimension / 2),
                PixelsPerUnit = 1
            };

            var spriteAssetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Sprite, spriteAssetContent);

            var spriteAssetFileName = AssetFileUtils.AppendExtension($"{Path.GetFileNameWithoutExtension(textureAssetFilePath)}.sprite");
            var spriteAssetFilePath = Path.Combine(directoryPath, spriteAssetFileName);
            spriteAssetData.Save(spriteAssetFilePath);

            return spriteAssetFilePath;
        }
    }
}