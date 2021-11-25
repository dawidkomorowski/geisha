using System;
using System.Drawing;
using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Editor.CreateAsset.Model
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
            if (CreateSpriteAssetUtils.IsTextureFile(textureFile.Path))
            {
                CreateSpriteFromTextureFile(textureFile);
            }
            else if (CreateSpriteAssetUtils.IsTextureAssetFile(textureFile.Path))
            {
                CreateSpriteFromTextureAssetFile(textureFile);
            }
            else
            {
                throw new ArgumentException(
                    $"Given file is neither texture asset file ({AssetFileUtils.Extension}) nor texture file (i.e. .png).", nameof(textureFile));
            }
        }

        private void CreateSpriteFromTextureFile(IProjectFile textureFile)
        {
            var textureAssetFile = _createTextureAssetService.CreateTextureAsset(textureFile);
            CreateSpriteFromTextureAssetFile(textureAssetFile);
        }

        private static void CreateSpriteFromTextureAssetFile(IProjectFile textureAssetFile)
        {
            var textureAssetData = AssetData.Load(textureAssetFile.Path);
            var textureAssetContent = textureAssetData.ReadJsonContent<TextureAssetContent>();

            if (textureAssetContent.TextureFilePath == null)
                throw new ArgumentException($"{nameof(TextureAssetContent)}.{nameof(TextureAssetContent.TextureFilePath)} cannot be null.");

            var textureImageFilePath = PathUtils.GetSiblingPath(textureAssetFile.Path, textureAssetContent.TextureFilePath);
            Vector2 spriteDimension;
            using (var bitmapImage = Image.FromFile(textureImageFilePath))
            {
                spriteDimension = new Vector2(bitmapImage.Width, bitmapImage.Height);
            }

            var spriteAssetFileName = AssetFileUtils.AppendExtension($"{Path.GetFileNameWithoutExtension(textureAssetFile.Name)}.sprite");
            var folder = textureAssetFile.ParentFolder;

            var spriteAssetContent = new SpriteAssetContent
            {
                TextureAssetId = textureAssetData.AssetId.Value,
                SourceUV = SerializableVector2.FromVector2(Vector2.Zero),
                SourceDimension = SerializableVector2.FromVector2(spriteDimension),
                SourceAnchor = SerializableVector2.FromVector2(spriteDimension / 2),
                PixelsPerUnit = 1
            };

            var spriteAssetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Sprite, spriteAssetContent);
            using var memoryStream = new MemoryStream();
            spriteAssetData.Save(memoryStream);
            memoryStream.Position = 0;
            folder.AddFile(spriteAssetFileName, memoryStream);
        }
    }
}