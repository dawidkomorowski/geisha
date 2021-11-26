using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Tools;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateTextureAssetService
    {
        void CreateTextureAsset(IProjectFile textureFile);
    }

    internal sealed class CreateTextureAssetService : ICreateTextureAssetService
    {
        private readonly IAssetToolCreateTextureAsset _assetToolCreateTextureAsset;

        public CreateTextureAssetService(IAssetToolCreateTextureAsset assetToolCreateTextureAsset)
        {
            _assetToolCreateTextureAsset = assetToolCreateTextureAsset;
        }

        public void CreateTextureAsset(IProjectFile textureFile)
        {
            var textureAssetFilePath = _assetToolCreateTextureAsset.Create(textureFile.Path);
            var parentFolder = textureFile.ParentFolder;
            parentFolder.IncludeFile(Path.GetFileName(textureAssetFilePath));
        }
    }

    internal interface IAssetToolCreateTextureAsset
    {
        string Create(string textureFilePath);
    }

    internal sealed class AssetToolCreateTextureAsset : IAssetToolCreateTextureAsset
    {
        public string Create(string textureFilePath) => AssetTool.CreateTextureAsset(textureFilePath);
    }
}