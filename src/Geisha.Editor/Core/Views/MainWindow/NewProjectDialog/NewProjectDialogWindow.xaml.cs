using Geisha.Editor.Core.ViewModels.MainWindow.NewProjectDialog;
using Geisha.Editor.Core.Views.Infrastructure;

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