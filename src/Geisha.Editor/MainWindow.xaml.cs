using System;
using System.Windows;
using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;

namespace Geisha.Editor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel viewModel) : this()
        {
            DataContext = viewModel;

            viewModel.NewProjectDialogRequested += ViewModelOnNewProjectDialogRequested;
            viewModel.OpenFileDialogRequested += ViewModelOnOpenFileDialogRequested;
            viewModel.CloseRequested += ViewModelOnCloseRequested;
        }

        private void ViewModelOnNewProjectDialogRequested(object sender, MainViewModel.NewProjectDialogRequestedEventArgs e)
        {
            var newProjectDialogWindow = new NewProjectDialogWindow(e.ViewModel)
            {
                Owner = this
            };
            newProjectDialogWindow.ShowDialog();
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