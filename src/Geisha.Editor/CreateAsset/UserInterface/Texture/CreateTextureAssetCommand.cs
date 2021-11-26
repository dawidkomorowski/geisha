using System;
using System.Windows.Input;
using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateAsset.UserInterface.Texture
{
    internal sealed class CreateTextureAssetCommand : ICommand
    {
        private readonly ICreateTextureAssetService _createTextureAssetService;
        private readonly IProjectFile _textureFile;

        public CreateTextureAssetCommand(ICreateTextureAssetService createTextureAssetService, IProjectFile textureFile)
        {
            _createTextureAssetService = createTextureAssetService;
            _textureFile = textureFile;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _createTextureAssetService.CreateTextureAsset(_textureFile);
        }

        public event EventHandler? CanExecuteChanged;
    }
}