using System;
using System.Windows.Input;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Sound
{
    internal sealed class CreateSoundAssetCommand : ICommand
    {
        private readonly ICreateSoundAssetService _createSoundAssetService;
        private readonly IProjectFile _soundFile;

        public CreateSoundAssetCommand(ICreateSoundAssetService createSoundAssetService, IProjectFile soundFile)
        {
            _createSoundAssetService = createSoundAssetService;
            _soundFile = soundFile;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _createSoundAssetService.CreateSoundAsset(_soundFile);
        }

        public event EventHandler? CanExecuteChanged;
    }
}