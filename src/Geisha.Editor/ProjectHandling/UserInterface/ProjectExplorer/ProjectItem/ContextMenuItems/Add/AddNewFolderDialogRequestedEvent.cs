using Geisha.Editor.Core;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public sealed class AddNewFolderDialogRequestedEvent : IEvent
    {
        public AddNewFolderDialogRequestedEvent(AddNewFolderDialogViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public AddNewFolderDialogViewModel ViewModel { get; }
    }
}