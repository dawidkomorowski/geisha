using System;
using System.Windows.Input;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Texture
{
    internal sealed class CreateTextureAssetCommand : ICommand
    {
        private readonly ICreateTextureAssetService _createTextureAssetService;
        private readonly IProjectFile _sourceTextureFile;

        public CreateTextureAssetCommand(ICreateTextureAssetService createTextureAssetService, IProjectFile sourceTextureFile)
        {
            _createTextureAssetService = createTextureAssetService;
            _sourceTextureFile = sourceTextureFile;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _createTextureAssetService.CreateTextureAsset(_sourceTextureFile);
        }

        public event EventHandler? CanExecuteChanged;
    }
}