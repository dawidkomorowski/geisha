using Geisha.Editor.Core.Views;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;

namespace Geisha.Editor
{
    public partial class MainWindow : GeishaEditorWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel) : this()
        {
            DataContext = viewModel;

            viewModel.NewProjectDialogRequested += ViewModelOnNewProjectDialogRequested;
        }

        private void ViewModelOnNewProjectDialogRequested(object sender, MainViewModel.NewProjectDialogRequestedEventArgs e)
        {
            var newProjectDialogWindow = new NewProjectDialogWindow(e.ViewModel)
            {
                Owner = this
            };
            newProjectDialogWindow.ShowDialog();
        }
    }
}