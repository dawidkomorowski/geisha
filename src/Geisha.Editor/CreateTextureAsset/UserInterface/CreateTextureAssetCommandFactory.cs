using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateTextureAsset.UserInterface
{
    internal interface ICreateTextureAssetCommandFactory
    {
        CreateTextureAssetCommand Create(IProjectFile sourceTextureFile);
    }

    internal class CreateTextureAssetCommandFactory : ICreateTextureAssetCommandFactory
    {
        private readonly ICreateTextureAssetService _createTextureAssetService;

        public CreateTextureAssetCommandFactory(ICreateTextureAssetService createTextureAssetService)
        {
            _createTextureAssetService = createTextureAssetService;
        }

        public CreateTextureAssetCommand Create(IProjectFile sourceTextureFile)
        {
            return new CreateTextureAssetCommand(_createTextureAssetService, sourceTextureFile);
        }
    }
}