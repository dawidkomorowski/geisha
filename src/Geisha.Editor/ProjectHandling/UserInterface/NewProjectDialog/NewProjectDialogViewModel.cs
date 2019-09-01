using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    public class NewProjectDialogViewModel : ViewModel, IWindowContext
    {
        private readonly IOpenFileDialogService _openFileDialogService;
        private readonly IProjectService _projectService;

        private string _projectName;
        private string _projectLocation;

        public IWindow Window { get; set; }

        public string ProjectName
        {
            get => _projectName;
            set => Set(ref _projectName, value);
        }

        public string ProjectLocation
        {
            get => _projectLocation;
            set => Set(ref _projectLocation, value);
        }

        public ICommand BrowseCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public NewProjectDialogViewModel(IOpenFileDialogService openFileDialogService, IProjectService projectService)
        {
            _openFileDialogService = openFileDialogService;
            _projectService = projectService;

            var okCommand = new RelayCommand(Ok, CanOk);
            Subscribe(nameof(ProjectName), () => okCommand.RaiseCanExecuteChanged());
            Subscribe(nameof(ProjectLocation), () => okCommand.RaiseCanExecuteChanged());

            BrowseCommand = new RelayCommand(Browse);
            OkCommand = okCommand;
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Browse()
        {
            var directoryPath = _openFileDialogService.AskForDirectoryPath();
            if (!string.IsNullOrEmpty(directoryPath))
            {
                ProjectLocation = directoryPath;
            }
        }

        private void Ok()
        {
            _projectService.CreateNewProject(_projectName, _projectLocation);
            Window.Close();
        }

        private bool CanOk()
        {
            return !(string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ProjectLocation));
        }

        private void Cancel()
        {
            Window.Close();
        }
    }
}