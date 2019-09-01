using System.Windows.Input;
using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public class AddNewFolderDialogViewModel : ViewModel, IWindowContext
    {
        private readonly IProjectFolder _folder;
        private readonly IProjectService _projectService;

        private string _folderName;

        public IWindow Window { get; set; }

        public string FolderName
        {
            get => _folderName;
            set => Set(ref _folderName, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public AddNewFolderDialogViewModel(IProjectFolder folder, IProjectService projectService)
        {
            _folder = folder;
            _projectService = projectService;

            var okCommand = new RelayCommand(Ok, CanOk);
            Subscribe(nameof(FolderName), () => okCommand.RaiseCanExecuteChanged());

            OkCommand = okCommand;
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Ok()
        {
            if (_folder == null)
            {
                _projectService.CurrentProject.AddFolder(FolderName);
            }
            else
            {
                _folder.AddFolder(FolderName);
            }

            Window.Close();
        }

        private bool CanOk()
        {
            return !string.IsNullOrWhiteSpace(FolderName);
        }

        private void Cancel()
        {
            Window.Close();
        }
    }
}