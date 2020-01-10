using System;
using System.Windows.Input;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSprite.UserInterface
{
    internal sealed class CreateSpriteCommand : ICommand
    {
        private readonly ICreateSpriteService _createSpriteService;
        private readonly IProjectFile _projectFile;

        public CreateSpriteCommand(ICreateSpriteService createSpriteService, IProjectFile projectFile)
        {
            _createSpriteService = createSpriteService;
            _projectFile = projectFile;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _createSpriteService.CreateSprite(_projectFile);
        }

        public event EventHandler CanExecuteChanged;
    }
}