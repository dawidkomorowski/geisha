using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Tools;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateSpriteAssetService
    {
        void CreateSpriteAsset(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteAssetService : ICreateSpriteAssetService
    {
        private readonly IAssetToolCreateSpriteAsset _assetToolCreateSpriteAsset;

        public CreateSpriteAssetService(IAssetToolCreateSpriteAsset assetToolCreateSpriteAsset)
        {
            _assetToolCreateSpriteAsset = assetToolCreateSpriteAsset;
        }

        public void CreateSpriteAsset(IProjectFile textureFile)
        {
            var (spriteAssetFilePaths, textureAssetFilePath) = _assetToolCreateSpriteAsset.Create(textureFile.Path);

            var parentFolder = textureFile.ParentFolder;
            parentFolder.IncludeFile(Path.GetFileName(spriteAssetFilePaths[0]));

            if (textureAssetFilePath != null)
            {
                parentFolder.IncludeFile(Path.GetFileName(textureAssetFilePath));
            }
        }
    }

    internal interface IAssetToolCreateSpriteAsset
    {
        (string[] spriteAssetFilePaths, string? textureAssetFilePath) Create(string textureFilePath);
    }

    internal sealed class AssetToolCreateSpriteAsset : IAssetToolCreateSpriteAsset
    {
        public (string[] spriteAssetFilePaths, string? textureAssetFilePath) Create(string textureFilePath) => AssetTool.CreateSpriteAsset(textureFilePath);
    }
}