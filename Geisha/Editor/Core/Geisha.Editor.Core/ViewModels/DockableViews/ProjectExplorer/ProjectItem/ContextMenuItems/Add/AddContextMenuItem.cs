using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public class AddContextMenuItem : ContextMenuItem
    {
        private readonly IProjectItem _projectItem;
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;
        private readonly IWindow _window;

        public AddContextMenuItem(IProjectItem projectItem,
            IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory, IWindow window) : base("Add")
        {
            _projectItem = projectItem;

            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
            _window = window;

            Items.Add(new ContextMenuItem("New Folder", new RelayCommand(NewFolder)));
        }

        private void NewFolder()
        {
            var viewModel = _addNewFolderDialogViewModelFactory.Create(_projectItem);
            _window.ShowModalChildWindow(viewModel);
        }
    }
}