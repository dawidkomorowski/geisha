using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public class AddContextMenuItem : ContextMenuItem
    {
        private readonly IProjectFolder _folder;
        private readonly IAddNewFolderDialogViewModelFactory _addNewFolderDialogViewModelFactory;
        private readonly IEventBus _eventBus;

        public AddContextMenuItem(
            IProjectFolder folder,
            IAddNewFolderDialogViewModelFactory addNewFolderDialogViewModelFactory,
            IEventBus eventBus) : base("Add")
        {
            _folder = folder;
            _addNewFolderDialogViewModelFactory = addNewFolderDialogViewModelFactory;
            _eventBus = eventBus;

            Items.Add(new ContextMenuItem("New Folder", new RelayCommand(NewFolder)));
        }

        private void NewFolder()
        {
            var viewModel = _addNewFolderDialogViewModelFactory.Create(_folder);
            _eventBus.SendEvent(new AddNewFolderDialogRequestedEvent(viewModel));
        }
    }
}