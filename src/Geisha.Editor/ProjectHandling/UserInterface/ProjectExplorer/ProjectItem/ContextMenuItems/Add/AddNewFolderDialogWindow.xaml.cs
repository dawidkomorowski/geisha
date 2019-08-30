using Geisha.Editor.Core.Views.Infrastructure;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
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