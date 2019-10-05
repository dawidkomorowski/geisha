using System;
using System.Windows;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add.NewFolder
{
    internal partial class AddNewFolderDialogWindow : Window
    {
        public AddNewFolderDialogWindow()
        {
            InitializeComponent();
        }

        public AddNewFolderDialogWindow(AddNewFolderDialogViewModel viewModel) : this()
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