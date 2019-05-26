using System.Windows.Input;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public class AddNewFolderDialogViewModel : ViewModel, IWindowContext
    {
        private readonly IProjectItem _projectItem;
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

        public AddNewFolderDialogViewModel(IProjectItem projectItem, IProjectService projectService)
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