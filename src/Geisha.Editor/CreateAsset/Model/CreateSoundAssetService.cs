using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Tools;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateSoundAssetService
    {
        void CreateSoundAsset(IProjectFile soundFile);
    }

    internal sealed class CreateSoundAssetService : ICreateSoundAssetService
    {
        private readonly IAssetToolCreateSoundAsset _assetToolCreateSoundAsset;

        public CreateSoundAssetService(IAssetToolCreateSoundAsset assetToolCreateSoundAsset)
        {
            _assetToolCreateSoundAsset = assetToolCreateSoundAsset;
        }

        public void CreateSoundAsset(IProjectFile soundFile)
        {
            var soundAssetFilePath = _assetToolCreateSoundAsset.Create(soundFile.Path);
            var parentFolder = soundFile.ParentFolder;
            parentFolder.IncludeFile(Path.GetFileName(soundAssetFilePath));
        }
    }

    internal interface IAssetToolCreateSoundAsset
    {
        string Create(string soundFilePath);
    }

    internal sealed class AssetToolCreateSoundAsset : IAssetToolCreateSoundAsset
    {
        public string Create(string soundFilePath) => AssetTool.CreateSoundAsset(soundFilePath);
    }
}