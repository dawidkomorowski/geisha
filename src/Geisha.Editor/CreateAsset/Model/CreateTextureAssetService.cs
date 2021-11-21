using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Tools;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateTextureAssetService
    {
        IProjectFile CreateTextureAsset(IProjectFile sourceTextureFile);
    }

    internal sealed class CreateTextureAssetService : ICreateTextureAssetService
    {
        private readonly IAssetToolCreateTextureAsset _assetToolCreateTextureAsset;

        public CreateTextureAssetService(IAssetToolCreateTextureAsset assetToolCreateTextureAsset)
        {
            _assetToolCreateTextureAsset = assetToolCreateTextureAsset;
        }

        public IProjectFile CreateTextureAsset(IProjectFile sourceTextureFile)
        {
            var textureAssetFilePath = _assetToolCreateTextureAsset.Create(sourceTextureFile.Path);
            var parentFolder = sourceTextureFile.ParentFolder;
            return parentFolder.IncludeFile(Path.GetFileName(textureAssetFilePath));
        }
    }

    internal interface IAssetToolCreateTextureAsset
    {
        string Create(string sourceTextureFilePath);
    }

    internal sealed class AssetToolCreateTextureAsset : IAssetToolCreateTextureAsset
    {
        public string Create(string sourceTextureFilePath) => AssetTool.CreateTextureAsset(sourceTextureFilePath);
    }
}