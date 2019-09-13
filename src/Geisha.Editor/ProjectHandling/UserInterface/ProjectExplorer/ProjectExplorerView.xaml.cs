using System.Windows;
using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.NewFolder;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
{
    public partial class ProjectExplorerView : UserControl, IView
    {
        public ProjectExplorerView()
        {
            InitializeComponent();

            EventBus.Default.RegisterEventHandler<AddNewFolderDialogRequestedEvent>(AddNewFolderDialogRequestedEventHandler);
            EventBus.Default.RegisterEventHandler<AddSceneDialogRequestedEvent>(AddSceneDialogRequestedEventHandler);
        }

        private void AddNewFolderDialogRequestedEventHandler(AddNewFolderDialogRequestedEvent @event)
        {
            var addNewFolderDialogWindow = new AddNewFolderDialogWindow(@event.ViewModel)
            {
                Owner = Window.GetWindow(this)
            };
            addNewFolderDialogWindow.ShowDialog();
        }

        private void AddSceneDialogRequestedEventHandler(AddSceneDialogRequestedEvent @event)
        {
            var addSceneDialogWindow = new AddSceneDialogWindow(@event.ViewModel)
            {
                Owner = Window.GetWindow(this)
            };
            addSceneDialogWindow.ShowDialog();
        }
    }
}