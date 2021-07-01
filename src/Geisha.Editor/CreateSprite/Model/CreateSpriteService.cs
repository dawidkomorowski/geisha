using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using Geisha.Common;
using Geisha.Common.FileSystem;
using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Editor.CreateSprite.Model
{
    internal interface ICreateSpriteService
    {
        void CreateSprite(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteService : ICreateSpriteService
    {
        private readonly ICreateTextureService _createTextureService;

        public CreateSpriteService(ICreateTextureService createTextureService)
        {
            _createTextureService = createTextureService;
        }

        public void CreateSprite(IProjectFile textureFile)
        {
            if (CreateSpriteUtils.IsTextureDataFile(textureFile.Extension))
            {
                CreateSpriteFromTextureDataFile(textureFile);
            }
            else if (CreateSpriteUtils.IsTextureMetadataFile(textureFile.Extension))
            {
                CreateSpriteFromTextureMetadataFile(textureFile);
            }
            else
            {
                throw new ArgumentException(
                    $"Given file is neither texture metadata file ({RenderingFileExtensions.Texture}) nor texture data file (i.e. .png).", nameof(textureFile));
            }
        }

        private void CreateSpriteFromTextureDataFile(IProjectFile textureDataFile)
        {
            var textureMetadataFile = _createTextureService.CreateTexture(textureDataFile);
            CreateSpriteFromTextureMetadataFile(textureMetadataFile);
        }

        private static void CreateSpriteFromTextureMetadataFile(IProjectFile textureMetadataFile)
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