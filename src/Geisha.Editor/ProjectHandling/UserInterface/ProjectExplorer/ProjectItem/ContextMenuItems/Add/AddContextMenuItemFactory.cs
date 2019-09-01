using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddContextMenuItemFactory
    {
        AddContextMenuItem Create(IProjectFolder folder, IWindow window);
    }

    public class AddContextMenuItemFactory : IAddContextMenuItemFactory
    {
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;

        public AddContextMenuItemFactory(IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory)
        {
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
        }

        public AddContextMenuItem Create(IProjectFolder folder, IWindow window)
        {
            return new AddContextMenuItem(folder, _addNewFolderDialogViewModelFactory, window);
        }
    }
}