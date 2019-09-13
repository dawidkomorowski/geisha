using Geisha.Editor.Core;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder
{
    internal sealed class AddNewFolderDialogRequestedEvent : IEvent
    {
        public AddNewFolderDialogRequestedEvent(AddNewFolderDialogViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public AddNewFolderDialogViewModel ViewModel { get; }
    }
}