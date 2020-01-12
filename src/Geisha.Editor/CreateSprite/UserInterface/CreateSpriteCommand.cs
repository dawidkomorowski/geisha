using System;
using System.Windows.Input;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSprite.UserInterface
{
    internal sealed class CreateSpriteCommand : ICommand
    {
        private readonly ICreateSpriteService _createSpriteService;
        private readonly IProjectFile _textureFile;

        public CreateSpriteCommand(ICreateSpriteService createSpriteService, IProjectFile textureFile)
        {
            _createSpriteService = createSpriteService;
            _textureFile = textureFile;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _createSpriteService.CreateSprite(_textureFile);
        }

        public event EventHandler CanExecuteChanged;
    }
}