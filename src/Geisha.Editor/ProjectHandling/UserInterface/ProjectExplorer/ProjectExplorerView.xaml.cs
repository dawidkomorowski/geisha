using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
{
    public partial class ProjectExplorerView : UserControl, IView
    {
        public ProjectExplorerView()
        {
            InitializeComponent();

            EventBus.Default.RegisterEventHandler<AddNewFolderDialogRequestedEvent>(AddNewFolderDialogRequestedEventHandler);
        }

        private void AddNewFolderDialogRequestedEventHandler(AddNewFolderDialogRequestedEvent @event)
        {
            var addNewFolderDialogWindow = new AddNewFolderDialogWindow(@event.ViewModel)
            {
                Owner = Window.GetWindow(this)
            };
            addNewFolderDialogWindow.ShowDialog();
        }
    }
}