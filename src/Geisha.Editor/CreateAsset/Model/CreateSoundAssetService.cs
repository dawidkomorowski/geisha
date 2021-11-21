using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Tools;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateSoundAssetService
    {
        void CreateSoundAsset(IProjectFile sourceSoundFile);
    }

    internal sealed class CreateSoundAssetService : ICreateSoundAssetService
    {
        private readonly IAssetToolCreateSoundAsset _assetToolCreateSoundAsset;

        public CreateSoundAssetService(IAssetToolCreateSoundAsset assetToolCreateSoundAsset)
        {
            _assetToolCreateSoundAsset = assetToolCreateSoundAsset;
        }

        public void CreateSoundAsset(IProjectFile sourceSoundFile)
        {
            var soundAssetFilePath = _assetToolCreateSoundAsset.Create(sourceSoundFile.Path);
            var parentFolder = sourceSoundFile.ParentFolder;
            parentFolder.IncludeFile(Path.GetFileName(soundAssetFilePath));
        }
    }

    internal interface IAssetToolCreateSoundAsset
    {
        string Create(string sourceSoundFilePath);
    }

    internal class AssetToolCreateSoundAsset : IAssetToolCreateSoundAsset
    {
        public string Create(string sourceSoundFilePath) => AssetTool.CreateSoundAsset(sourceSoundFilePath);
    }
}