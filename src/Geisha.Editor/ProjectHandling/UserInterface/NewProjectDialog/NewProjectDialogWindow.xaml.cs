using System;
using System.Windows;
using Geisha.Editor.Core;

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

            viewModel.OpenFileDialogRequested += ViewModelOnOpenFileDialogRequested;
            viewModel.CloseRequested += ViewModelOnCloseRequested;
        }

        private void ViewModelOnOpenFileDialogRequested(object sender, OpenFileDialogEventArgs e)
        {
            OpenFileDialog.HandleEvent(e, this);
        }

        private void ViewModelOnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}