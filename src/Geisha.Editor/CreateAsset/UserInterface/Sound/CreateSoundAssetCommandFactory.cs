using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Sound
{
    internal interface ICreateSoundAssetCommandFactory
    {
        CreateSoundAssetCommand Create(IProjectFile soundFile);
    }

    internal sealed class CreateSoundAssetCommandFactory : ICreateSoundAssetCommandFactory
    {
        private readonly ICreateSoundAssetService _createSoundAssetService;

        public CreateSoundAssetCommandFactory(ICreateSoundAssetService createSoundAssetService)
        {
            _createSoundAssetService = createSoundAssetService;
        }

        public CreateSoundAssetCommand Create(IProjectFile soundFile)
        {
            return new CreateSoundAssetCommand(_createSoundAssetService, soundFile);
        }
    }
}