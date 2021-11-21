using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateTextureAssetService
    {
        IProjectFile CreateTextureAsset(IProjectFile sourceTextureFile);
    }

    internal sealed class CreateTextureAssetService : ICreateTextureAssetService
    {
        public IProjectFile CreateTextureAsset(IProjectFile sourceTextureFile)
        {
            var textureAssetFileName = AssetFileUtils.AppendExtension(Path.GetFileNameWithoutExtension(sourceTextureFile.Name));
            var folder = sourceTextureFile.ParentFolder;

            var textureAssetContent = new TextureAssetContent
            {
                TextureFilePath = sourceTextureFile.Name
            };

            using var memoryStream = new MemoryStream();
            var assetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), RenderingAssetTypes.Texture, textureAssetContent);
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            return folder.AddFile(textureAssetFileName, memoryStream);
        }
    }
}