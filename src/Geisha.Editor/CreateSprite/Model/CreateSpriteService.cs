using System.Drawing;
using System.IO;
using System.Text;
using Geisha.Common.FileSystem;
using Geisha.Common.Math;
using Geisha.Common.Math.Serialization;
using Geisha.Common.Serialization;
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
        private readonly IJsonSerializer _jsonSerializer;

        public CreateSpriteService(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public void CreateSprite(IProjectFile textureFile)
        {
            var serializedTextureFileContent = File.ReadAllText(textureFile.Path);
            var textureFileContent = _jsonSerializer.Deserialize<TextureFileContent>(serializedTextureFileContent);

            var textureImageFilePath = PathUtils.GetSiblingPath(textureFile.Path, textureFileContent.TextureFilePath);
            Vector2 spriteDimension;
            using (var bitmapImage = Image.FromFile(textureImageFilePath))
            {
                spriteDimension = new Vector2(bitmapImage.Width, bitmapImage.Height);
            }

            var spriteFileName = $"{Path.GetFileNameWithoutExtension(textureFile.Name)}{RenderingFileExtensions.Sprite}";
            var folder = textureFile.ParentFolder;

            var spriteFileContent = new SpriteFileContent
            {
                AssetId = AssetId.CreateUnique().Value,
                TextureAssetId = textureFileContent.AssetId,
                SourceUV = SerializableVector2.FromVector2(Vector2.Zero),
                SourceDimension = SerializableVector2.FromVector2(spriteDimension),
                SourceAnchor = SerializableVector2.FromVector2(spriteDimension / 2),
                PixelsPerUnit = 1
            };

            var json = _jsonSerializer.Serialize(spriteFileContent);

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 512, true))
                {
                    streamWriter.Write(json);
                }

                memoryStream.Position = 0;
                folder.AddFile(spriteFileName, memoryStream);
            }
        }
    }
}