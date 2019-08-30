using Geisha.Editor.Core.Views.Infrastructure;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.Core.Views.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    [ViewModel(typeof(AddNewFolderDialogViewModel))]
    public partial class AddNewFolderDialogWindow : GeishaEditorWindow
    {
        public AddNewFolderDialogWindow()
        {
            InitializeComponent();
        }
    }
}