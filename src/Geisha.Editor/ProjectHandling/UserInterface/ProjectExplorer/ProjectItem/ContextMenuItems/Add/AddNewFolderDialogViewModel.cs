using System.Windows.Input;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public class AddNewFolderDialogViewModel : ViewModel, IWindowContext
    {
        private readonly IProjectItemObsolete _projectItem;
        private readonly IProjectServiceObsolete _projectService;

        private string _folderName;

        public IWindow Window { get; set; }

        public string FolderName
        {
            get => _folderName;
            set => Set(ref _folderName, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public AddNewFolderDialogViewModel(IProjectItemObsolete projectItem, IProjectServiceObsolete projectService)
        {
            _projectItem = projectItem;
            _projectService = projectService;

            var okCommand = new RelayCommand(Ok, CanOk);
            Subscribe(nameof(FolderName), () => okCommand.RaiseCanExecuteChanged());

            OkCommand = okCommand;
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Ok()
        {
            if (_projectItem == null)
            {
                _projectService.CurrentProject.AddFolder(FolderName);
            }
            else
            {
                _projectItem.AddFolder(FolderName);
            }
            _projectService.SaveProject();

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