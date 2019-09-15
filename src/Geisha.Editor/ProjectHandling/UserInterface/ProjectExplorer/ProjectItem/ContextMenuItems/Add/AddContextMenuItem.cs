using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    internal sealed class AddContextMenuItem : ContextMenuItem
    {
        private readonly IEventBus _eventBus;
        private readonly IProject _project;
        private readonly IProjectFolder _folder;
        private readonly IAddSceneDialogViewModelFactory _addSceneDialogViewModelFactory;

        public AddContextMenuItem(
            IEventBus eventBus,
            IProject project,
            IProjectFolder folder,
            IAddSceneDialogViewModelFactory addSceneDialogViewModelFactory) : base("Add")
        {
            _eventBus = eventBus;
            _project = project;
            _folder = folder;
            _addSceneDialogViewModelFactory = addSceneDialogViewModelFactory;

            Items.Add(new ContextMenuItem("New Folder", new RelayCommand(NewFolder)));
            Items.Add(new ContextMenuItem("Scene", new RelayCommand(Scene)));
        }

        private void NewFolder()
        {
            AddNewFolderDialogViewModel viewModel;
            if (_project != null)
            {
                viewModel = new AddNewFolderDialogViewModel(_project);
            }
            else
            {
                viewModel = new AddNewFolderDialogViewModel(_folder);
            }

            _eventBus.SendEvent(new AddNewFolderDialogRequestedEvent(viewModel));
        }

        private void Scene()
        {
            var viewModel = _addSceneDialogViewModelFactory.Create();
            _eventBus.SendEvent(new AddSceneDialogRequestedEvent(viewModel));
        }
    }
}