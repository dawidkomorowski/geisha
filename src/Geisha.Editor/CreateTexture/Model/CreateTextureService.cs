using System.IO;
using System.Text.Json;
using Geisha.Common;
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
        public IProjectFile CreateTexture(IProjectFile sourceTextureFile)
        {
            var textureFileName = $"{Path.GetFileNameWithoutExtension(sourceTextureFile.Name)}{RenderingFileExtensions.Texture}";
            var folder = sourceTextureFile.ParentFolder;

            var textureFileContent = new TextureFileContent
            {
                AssetId = AssetId.CreateUnique().Value,
                TextureFilePath = sourceTextureFile.Name
            };

            var json = JsonSerializer.Serialize(textureFileContent);

            using var stream = json.ToStream();
            return folder.AddFile(textureFileName, stream);
        }
    }
}