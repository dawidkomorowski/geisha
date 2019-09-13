using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    internal sealed class AddContextMenuItem : ContextMenuItem
    {
        private readonly IEventBus _eventBus;
        private readonly IProjectFolder _folder;
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;
        private readonly IAddSceneDialogViewModelFactory _addSceneDialogViewModelFactory;

        public AddContextMenuItem(
            IEventBus eventBus,
            IProjectFolder folder,
            IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory,
            IAddSceneDialogViewModelFactory addSceneDialogViewModelFactory) : base("Add")
        {
            _folder = folder;
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
            _addSceneDialogViewModelFactory = addSceneDialogViewModelFactory;
            _eventBus = eventBus;

            Items.Add(new ContextMenuItem("New Folder", new RelayCommand(NewFolder)));
            Items.Add(new ContextMenuItem("Scene", new RelayCommand(Scene)));
        }

        private void NewFolder()
        {
            var viewModel = _addNewFolderDialogViewModelFactory.Create(_folder);
            _eventBus.SendEvent(new AddNewFolderDialogRequestedEvent(viewModel));
        }

        private void Scene()
        {
            var viewModel = _addSceneDialogViewModelFactory.Create();
            _eventBus.SendEvent(new AddSceneDialogRequestedEvent(viewModel));
        }
    }
}