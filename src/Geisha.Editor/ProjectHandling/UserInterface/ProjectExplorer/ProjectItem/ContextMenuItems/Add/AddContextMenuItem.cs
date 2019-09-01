using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public class AddContextMenuItem : ContextMenuItem
    {
        private readonly IProjectFolder _folder;
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;
        private readonly IWindow _window;

        public AddContextMenuItem(IProjectFolder folder,
            IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory, IWindow window) : base("Add")
        {
            _folder = folder;

            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
            _window = window;

            Items.Add(new ContextMenuItem("New Folder", new RelayCommand(NewFolder)));
        }

        private void NewFolder()
        {
            var viewModel = _addNewFolderDialogViewModelFactory.Create(_folder);
            _window.ShowModalChildWindow(viewModel);
        }
    }
}