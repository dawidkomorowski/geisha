using System;
using System.Windows;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add
{
    public partial class AddNewFolderDialogWindow : Window
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