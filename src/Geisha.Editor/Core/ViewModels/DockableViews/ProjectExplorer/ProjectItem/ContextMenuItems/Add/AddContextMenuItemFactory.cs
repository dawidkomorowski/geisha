using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add
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