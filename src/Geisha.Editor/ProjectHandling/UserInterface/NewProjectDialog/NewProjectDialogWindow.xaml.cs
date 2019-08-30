using Geisha.Editor.Core.Views.Infrastructure;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    [ViewModel(typeof(NewProjectDialogViewModel))]
    public partial class NewProjectDialogWindow : GeishaEditorWindow
    {
        public NewProjectDialogWindow()
        {
            InitializeComponent();
        }
    }
}