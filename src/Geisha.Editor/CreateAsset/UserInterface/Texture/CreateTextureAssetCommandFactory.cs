using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Texture
{
    internal interface ICreateTextureAssetCommandFactory
    {
        CreateTextureAssetCommand Create(IProjectFile textureFile);
    }

    internal class CreateTextureAssetCommandFactory : ICreateTextureAssetCommandFactory
    {
        private readonly ICreateTextureAssetService _createTextureAssetService;

        public CreateTextureAssetCommandFactory(ICreateTextureAssetService createTextureAssetService)
        {
            _createTextureAssetService = createTextureAssetService;
        }

        public CreateTextureAssetCommand Create(IProjectFile textureFile)
        {
            return new CreateTextureAssetCommand(_createTextureAssetService, textureFile);
        }
    }
}