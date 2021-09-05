using System.IO;
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
            var textureAssetFileName = $"{Path.GetFileNameWithoutExtension(sourceTextureFile.Name)}{AssetFileExtension.Value}";
            var folder = sourceTextureFile.ParentFolder;

            var textureFileContent = new TextureFileContent
            {
                TextureFilePath = sourceTextureFile.Name
            };

            using var memoryStream = new MemoryStream();
            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Texture, textureFileContent);
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            return folder.AddFile(textureAssetFileName, memoryStream);
        }
    }
}