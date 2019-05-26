using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using Geisha.Editor.Core.Views.Infrastructure;

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