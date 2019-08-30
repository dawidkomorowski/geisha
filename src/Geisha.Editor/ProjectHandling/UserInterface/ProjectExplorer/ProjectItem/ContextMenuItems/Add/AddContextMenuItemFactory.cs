using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Domain;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddContextMenuItemFactory
    {
        AddContextMenuItem Create(IProjectItem projectItem, IWindow window);
    }

    public class AddContextMenuItemFactory : IAddContextMenuItemFactory
    {
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;

        public AddContextMenuItemFactory(IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory)
        {
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
        }

        public AddContextMenuItem Create(IProjectItem projectItem, IWindow window)
        {
            return new AddContextMenuItem(projectItem, _addNewFolderDialogViewModelFactory, window);
        }
    }
}