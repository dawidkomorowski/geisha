using System;
using System.Windows;
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

        private void ViewModelOnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}