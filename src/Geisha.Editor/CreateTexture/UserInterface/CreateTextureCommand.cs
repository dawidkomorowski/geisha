using System;
using System.Windows.Input;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateTexture.UserInterface
{
    internal sealed class CreateTextureCommand : ICommand
    {
        private readonly ICreateTextureService _createTextureService;
        private readonly IProjectFile _projectFile;

        public CreateTextureCommand(ICreateTextureService createTextureService, IProjectFile projectFile)
        {
            _createTextureService = createTextureService;
            _projectFile = projectFile;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _createTextureService.CreateTexture(_projectFile);
        }

        public event EventHandler CanExecuteChanged;
    }
}