using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    internal interface IAddContextMenuItemFactory
    {
        AddContextMenuItem Create(IProjectFolder folder);
    }

    internal sealed class AddContextMenuItemFactory : IAddContextMenuItemFactory
    {
        private readonly IEventBus _eventBus;
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;
        private readonly IAddSceneDialogViewModelFactory _addSceneDialogViewModelFactory;

        public AddContextMenuItemFactory(
            IEventBus eventBus,
            IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory,
            IAddSceneDialogViewModelFactory addSceneDialogViewModelFactory)
        {
            _eventBus = eventBus;
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
            _addSceneDialogViewModelFactory = addSceneDialogViewModelFactory;
        }

        public AddContextMenuItem Create(IProjectFolder folder)
        {
            return new AddContextMenuItem(_eventBus, folder, _addNewFolderDialogViewModelFactory, _addSceneDialogViewModelFactory);
        }
    }
}