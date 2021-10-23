using System;
using System.Windows.Input;
using Geisha.Editor.CreateSpriteAsset.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSpriteAsset.UserInterface
{
    internal sealed class CreateSpriteAssetCommand : ICommand
    {
        private readonly ICreateSpriteAssetService _createSpriteAssetService;
        private readonly IProjectFile _textureFile;

        public CreateSpriteAssetCommand(ICreateSpriteAssetService createSpriteAssetService, IProjectFile textureFile)
        {
            _createSpriteAssetService = createSpriteAssetService;
            _textureFile = textureFile;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _createSpriteAssetService.CreateSpriteAsset(_textureFile);
        }

        public event EventHandler? CanExecuteChanged;
    }
}