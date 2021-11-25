using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Sprite
{
    internal interface ICreateSpriteAssetCommandFactory
    {
        CreateSpriteAssetCommand Create(IProjectFile textureFile);
    }

    internal sealed class CreateSpriteAssetCommandFactory : ICreateSpriteAssetCommandFactory
    {
        private readonly ICreateSpriteAssetService _createSpriteAssetService;

        public CreateSpriteAssetCommandFactory(ICreateSpriteAssetService createSpriteAssetService)
        {
            _createSpriteAssetService = createSpriteAssetService;
        }

        public CreateSpriteAssetCommand Create(IProjectFile textureFile)
        {
            return new CreateSpriteAssetCommand(_createSpriteAssetService, textureFile);
        }
    }
}