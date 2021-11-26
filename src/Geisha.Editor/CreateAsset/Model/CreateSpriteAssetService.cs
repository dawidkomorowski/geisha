using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.Model
{
    internal interface ICreateSpriteAssetService
    {
        void CreateSpriteAsset(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteAssetService : ICreateSpriteAssetService
    {
        private readonly ICreateTextureAssetService _createTextureAssetService;

        public CreateSpriteAssetService(ICreateTextureAssetService createTextureAssetService)
        {
            _createTextureAssetService = createTextureAssetService;
        }

        public void CreateSpriteAsset(IProjectFile textureFile)
        {
        }
    }
}