using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    internal interface IAddContextMenuItemFactory
    {
        AddContextMenuItem Create(IProject project);
        AddContextMenuItem Create(IProjectFolder folder);
    }

    internal sealed class AddContextMenuItemFactory : IAddContextMenuItemFactory
    {
        private readonly IEventBus _eventBus;
        private readonly IAddSceneDialogViewModelFactory _addSceneDialogViewModelFactory;

        public AddContextMenuItemFactory(
            IEventBus eventBus,
            IAddSceneDialogViewModelFactory addSceneDialogViewModelFactory)
        {
            _eventBus = eventBus;
            _addSceneDialogViewModelFactory = addSceneDialogViewModelFactory;
        }

        public AddContextMenuItem Create(IProject project)
        {
            return new AddContextMenuItem(_eventBus, project, null, _addSceneDialogViewModelFactory);
        }

        public AddContextMenuItem Create(IProjectFolder folder)
        {
            return new AddContextMenuItem(_eventBus, null, folder, _addSceneDialogViewModelFactory);
        }
    }
}