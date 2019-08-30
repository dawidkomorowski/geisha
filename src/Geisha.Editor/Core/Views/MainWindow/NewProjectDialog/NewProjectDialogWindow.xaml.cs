using Geisha.Editor.Core.Views.Infrastructure;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;

namespace Geisha.Editor.Core.Views.MainWindow.NewProjectDialog
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