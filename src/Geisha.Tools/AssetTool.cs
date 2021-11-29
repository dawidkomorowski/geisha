using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Assets.Serialization;
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

        public static string CreateSoundAsset(string soundFilePath)
        {
            var soundFileExtension = Path.GetExtension(soundFilePath);
            if (!IsSupportedSoundFileFormat(soundFileExtension))
            {
                throw new ArgumentException($"Sound file format {soundFileExtension} is not supported.", nameof(soundFilePath));
            }

            var directoryPath = Path.GetDirectoryName(soundFilePath) ??
                                throw new ArgumentException("The path does not point to any file.", nameof(soundFilePath));

            var soundAssetContent = new SoundAssetContent
            {
                SoundFilePath = Path.GetFileName(soundFilePath)
            };

            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), AudioAssetTypes.Sound, soundAssetContent);

            var soundAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(soundFilePath));
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

        public static string CreateTextureAsset(string textureFilePath)
        {
            var textureFileExtension = Path.GetExtension(textureFilePath);
            if (!IsSupportedTextureFileFormat(textureFileExtension))
            {
                throw new ArgumentException($"Texture file format {textureFileExtension} is not supported.", nameof(textureFilePath));
            }

            var directoryPath = Path.GetDirectoryName(textureFilePath) ??
                                throw new ArgumentException("The path does not point to any file.", nameof(textureFilePath));

            var textureAssetContent = new TextureAssetContent
            {
                TextureFilePath = Path.GetFileName(textureFilePath)
            };

            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Texture, textureAssetContent);

            var textureAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(textureFilePath));
            var textureAssetFilePath = Path.Combine(directoryPath, textureAssetFileName);
            assetData.Save(textureAssetFilePath);

            return textureAssetFilePath;
        }

        public static bool CanCreateSpriteAssetFromFile(string filePath)
        {
            return IsTextureAssetFile(filePath) || IsTextureFile(filePath);
        }

        public static (string spriteAssetFilePath, string? textureAssetFilePath) CreateSpriteAsset(string textureAssetOrTextureFilePath)
        {
            if (IsTextureFile(textureAssetOrTextureFilePath))
            {
                var textureAssetFilePath = CreateTextureAsset(textureAssetOrTextureFilePath);
                var spriteAssetFilePath = CreateSpriteFromTextureAssetFile(textureAssetFilePath);
                return (spriteAssetFilePath, textureAssetFilePath);
            }

            if (IsTextureAssetFile(textureAssetOrTextureFilePath))
            {
                var spriteAssetFilePath = CreateSpriteFromTextureAssetFile(textureAssetOrTextureFilePath);
                return (spriteAssetFilePath, null);
            }

            throw new ArgumentException(
                $"Given file is neither texture asset file ({AssetFileUtils.Extension}) nor texture file (i.e. .png).", nameof(textureAssetOrTextureFilePath));
        }

        public static string CreateInputMappingAsset()
        {
            var inputMappingAssetContent = new InputMappingAssetContent
            {
                ActionMappings = new Dictionary<string, SerializableHardwareAction[]>
                {
                    ["Jump"] = new[]
                    {
                        new SerializableHardwareAction { Key = Key.Space },
                        new SerializableHardwareAction { Key = Key.Up }
                    }
                },
                AxisMappings = new Dictionary<string, SerializableHardwareAxis[]>
                {
                    ["MoveRight"] = new[]
                    {
                        new SerializableHardwareAxis { Key = Key.Right, Scale = 1.0 },
                        new SerializableHardwareAxis { Key = Key.Left, Scale = -1.0 }
                    }
                }
            };

            var inputMappingAssetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), InputAssetTypes.InputMapping, inputMappingAssetContent);

            var inputMappingAssetFileName = AssetFileUtils.AppendExtension("DefaultInputMapping");
            var inputMappingAssetFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputMappingAssetFileName);
            inputMappingAssetData.Save(inputMappingAssetFilePath);

            return inputMappingAssetFilePath;
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