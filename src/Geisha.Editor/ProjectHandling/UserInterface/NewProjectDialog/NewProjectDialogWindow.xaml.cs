using System;
using Geisha.Editor.Core.Views;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    [ViewModel(typeof(NewProjectDialogViewModel))]
    public partial class NewProjectDialogWindow : GeishaEditorWindow
    {
        public NewProjectDialogWindow()
        {
            InitializeComponent();
        }

        public NewProjectDialogWindow(NewProjectDialogViewModel viewModel) : this()
        {
            DataContext = viewModel;

            viewModel.CloseRequested += ViewModelOnCloseRequested;
        }

        private void ViewModelOnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}