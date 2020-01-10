using System.IO;
using System.Text;
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
            var spriteFileName = $"{Path.GetFileNameWithoutExtension(textureFile.Name)}{RenderingFileExtensions.Sprite}";
            var folder = textureFile.ParentFolder;

            var spriteFileContent = new SpriteFileContent
            {
                AssetId = AssetId.CreateUnique().Value
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