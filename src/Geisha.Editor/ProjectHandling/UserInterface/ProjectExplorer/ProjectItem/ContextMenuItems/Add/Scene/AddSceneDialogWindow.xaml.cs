using System;
using System.Windows;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene
{
    /// <summary>
    /// Interaction logic for AddSceneDialogWindow.xaml
    /// </summary>
    internal partial class AddSceneDialogWindow : Window
    {
        public AddSceneDialogWindow()
        {
            InitializeComponent();
        }

        public AddSceneDialogWindow(AddSceneDialogViewModel viewModel) : this()
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