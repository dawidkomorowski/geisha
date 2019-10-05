using Geisha.Editor.Core;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add.Scene
{
    internal sealed class AddSceneDialogRequestedEvent : IEvent
    {
        public AddSceneDialogRequestedEvent(AddSceneDialogViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public AddSceneDialogViewModel ViewModel { get; }
    }
}