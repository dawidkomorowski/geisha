using System;
using System.Windows.Input;
using Geisha.Editor.CreateSound.Model;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.CreateSound.UserInterface
{
    internal sealed class CreateSoundCommand : ICommand
    {
        private readonly ICreateSoundService _createSoundService;
        private readonly IProjectFile _sourceSoundFile;

        public CreateSoundCommand(ICreateSoundService createSoundService, IProjectFile sourceSoundFile)
        {
            _createSoundService = createSoundService;
            _sourceSoundFile = sourceSoundFile;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _createSoundService.CreateSound(_sourceSoundFile);
        }

        public event EventHandler? CanExecuteChanged;
    }
}