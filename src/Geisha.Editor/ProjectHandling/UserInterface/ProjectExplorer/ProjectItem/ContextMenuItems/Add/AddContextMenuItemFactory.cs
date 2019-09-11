using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public interface IAddContextMenuItemFactory
    {
        AddContextMenuItem Create(IProjectFolder folder);
    }

    public class AddContextMenuItemFactory : IAddContextMenuItemFactory
    {
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;
        private readonly IEventBus _eventBus;

        public AddContextMenuItemFactory(IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory, IEventBus eventBus)
        {
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
            _eventBus = eventBus;
        }

        public AddContextMenuItem Create(IProjectFolder folder)
        {
            return new AddContextMenuItem(folder, _addNewFolderDialogViewModelFactory, _eventBus);
        }
    }
}