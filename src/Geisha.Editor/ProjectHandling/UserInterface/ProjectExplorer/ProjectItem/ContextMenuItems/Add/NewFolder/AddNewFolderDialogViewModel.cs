using System;
using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder
{
    internal sealed class AddNewFolderDialogViewModel : ViewModel
    {
        private readonly IProjectFolder _folder;
        private readonly IProjectService _projectService;

        private readonly IProperty<string> _folderName;

        public string FolderName
        {
            get => _folderName.Get();
            set => _folderName.Set(value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler CloseRequested;

        public AddNewFolderDialogViewModel(IProjectFolder folder, IProjectService projectService)
        {
            _folder = folder;
            _projectService = projectService;

            var okCommand = new RelayCommand(Ok, CanOk);
            OkCommand = okCommand;
            CancelCommand = new RelayCommand(Cancel);

            _folderName = CreateProperty<string>(nameof(FolderName));
            _folderName.Subscribe(_ => okCommand.RaiseCanExecuteChanged());
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

            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanOk()
        {
            return !string.IsNullOrWhiteSpace(FolderName);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}