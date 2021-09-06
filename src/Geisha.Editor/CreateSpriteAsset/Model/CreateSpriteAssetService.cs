using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Editor.CreateSpriteAsset.Model
{
    internal interface ICreateSpriteAssetService
    {
        void CreateSpriteAsset(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteAssetService : ICreateSpriteAssetService
    {
        private readonly ICreateTextureAssetService _createTextureAssetService;

        public CreateSpriteAssetService(ICreateTextureAssetService createTextureAssetService)
        {
            _createTextureAssetService = createTextureAssetService;
        }

        public void CreateSpriteAsset(IProjectFile textureFile)
        {
            if (CreateSpriteAssetUtils.IsTextureFile(textureFile.Extension))
            {
                CreateSpriteFromTextureFile(textureFile);
            }
            else if (CreateSpriteAssetUtils.IsTextureAssetFile(textureFile.Extension))
            {
                CreateSpriteFromTextureAssetFile(textureFile);
            }
            else
            {
                throw new ArgumentException(
                    $"Given file is neither texture metadata file ({RenderingFileExtensions.Texture}) nor texture data file (i.e. .png).", nameof(textureFile));
            }
        }

        private void CreateSpriteFromTextureFile(IProjectFile textureDataFile)
        {
            var textureMetadataFile = _createTextureAssetService.CreateTextureAsset(textureDataFile);
            CreateSpriteFromTextureAssetFile(textureMetadataFile);
        }

        private static void CreateSpriteFromTextureAssetFile(IProjectFile textureMetadataFile)
        {
            var serializedTextureFileContent = File.ReadAllText(textureMetadataFile.Path);
            var textureFileContent = JsonSerializer.Deserialize<TextureFileContent>(serializedTextureFileContent);

            if (textureFileContent is null) throw new ArgumentException($"{nameof(TextureFileContent)} cannot be null.");

            if (textureFileContent.TextureFilePath == null)
                throw new ArgumentException($"{nameof(TextureFileContent)}.{nameof(TextureFileContent.TextureFilePath)} cannot be null.");

            var textureImageFilePath = PathUtils.GetSiblingPath(textureMetadataFile.Path, textureFileContent.TextureFilePath);
            Vector2 spriteDimension;
            using (var bitmapImage = Image.FromFile(textureImageFilePath))
            {
                spriteDimension = new Vector2(bitmapImage.Width, bitmapImage.Height);
            }

            var spriteFileName = $"{Path.GetFileNameWithoutExtension(textureMetadataFile.Name)}{RenderingFileExtensions.Sprite}";
            var folder = textureMetadataFile.ParentFolder;

            var spriteFileContent = new SpriteFileContent
            {
                AssetId = AssetId.CreateUnique().Value,
                TextureAssetId = textureFileContent.AssetId,
                SourceUV = SerializableVector2.FromVector2(Vector2.Zero),
                SourceDimension = SerializableVector2.FromVector2(spriteDimension),
                SourceAnchor = SerializableVector2.FromVector2(spriteDimension / 2),
                PixelsPerUnit = 1
            };

            var json = JsonSerializer.Serialize(spriteFileContent);

            using (var stream = json.ToStream())
            {
                folder.AddFile(spriteFileName, stream);
            }
        }
    }
}