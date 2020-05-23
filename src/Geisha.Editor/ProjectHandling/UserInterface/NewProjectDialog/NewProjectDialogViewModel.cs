using System;
using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    public class NewProjectDialogViewModel : ViewModel
    {
        private readonly IProjectService _projectService;

        private readonly IProperty<string> _projectName;
        private readonly IProperty<string> _projectLocation;

        public string ProjectName
        {
            get => _projectName.Get();
            set => _projectName.Set(value);
        }

        public string ProjectLocation
        {
            get => _projectLocation.Get();
            set => _projectLocation.Set(value);
        }

        public ICommand BrowseCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<OpenFileDialogEventArgs>? OpenFileDialogRequested;
        public event EventHandler? CloseRequested;

        public NewProjectDialogViewModel(IProjectService projectService)
        {
            _projectService = projectService;

            BrowseCommand = RelayCommand.Create(Browse);
            var okCommand = RelayCommand.Create(Ok, CanOk);
            OkCommand = okCommand;
            CancelCommand = RelayCommand.Create(Cancel);

            _projectName = CreateProperty<string>(nameof(ProjectName));
            _projectLocation = CreateProperty<string>(nameof(ProjectLocation));

            _projectName.Subscribe(_ => okCommand.RaiseCanExecuteChanged());
            _projectLocation.Subscribe(_ => okCommand.RaiseCanExecuteChanged());
        }

        private void Browse()
        {
            OpenFileDialogRequested?.Invoke(this, OpenFileDialogEventArgs.AskForDirectoryPath()
                .AndContinueWith(directoryPath =>
                {
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        ProjectLocation = directoryPath;
                    }
                }));
        }

        private void Ok()
        {
            _projectService.CreateNewProject(ProjectName, ProjectLocation);
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanOk()
        {
            return !(string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ProjectLocation));
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}