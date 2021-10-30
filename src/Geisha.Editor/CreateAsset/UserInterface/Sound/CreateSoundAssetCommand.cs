using System;
using System.Windows.Input;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Sound
{
    internal sealed class CreateSoundAssetCommand : ICommand
    {
        private readonly ICreateSoundAssetService _createSoundAssetService;
        private readonly IProjectFile _sourceSoundFile;

        public CreateSoundAssetCommand(ICreateSoundAssetService createSoundAssetService, IProjectFile sourceSoundFile)
        {
            _createSoundAssetService = createSoundAssetService;
            _sourceSoundFile = sourceSoundFile;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _createSoundAssetService.CreateSoundAsset(_sourceSoundFile);
        }

        public event EventHandler? CanExecuteChanged;
    }
}