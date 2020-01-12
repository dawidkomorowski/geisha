using System.IO;
using System.Text;
using Geisha.Common.Serialization;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Editor.CreateTexture.Model
{
    internal interface ICreateTextureService
    {
        IProjectFile CreateTexture(IProjectFile sourceTextureFile);
    }

    internal sealed class CreateTextureService : ICreateTextureService
    {
        private readonly IJsonSerializer _jsonSerializer;

        public CreateTextureService(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public IProjectFile CreateTexture(IProjectFile sourceTextureFile)
        {
            var textureFileName = $"{Path.GetFileNameWithoutExtension(sourceTextureFile.Name)}{RenderingFileExtensions.Texture}";
            var folder = sourceTextureFile.ParentFolder;

            var textureFileContent = new TextureFileContent
            {
                AssetId = AssetId.CreateUnique().Value,
                TextureFilePath = sourceTextureFile.Name
            };

            var json = _jsonSerializer.Serialize(textureFileContent);

            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 512, true))
                {
                    streamWriter.Write(json);
                }

                memoryStream.Position = 0;
                return folder.AddFile(textureFileName, memoryStream);
            }
        }
    }
}