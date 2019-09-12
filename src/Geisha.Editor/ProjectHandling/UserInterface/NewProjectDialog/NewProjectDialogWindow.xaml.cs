using System;
using System.Windows;

namespace Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog
{
    public partial class NewProjectDialogWindow : Window
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