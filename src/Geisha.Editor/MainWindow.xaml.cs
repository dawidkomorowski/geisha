using System;
using System.IO;
using System.Windows;
using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Geisha.Editor
{
    internal partial class MainWindow : Window
    {
        private const string EditorLayoutXmlFilePath = "GeishaEditorLayout.xml";

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

        public void LoadLayout()
        {
            if (File.Exists(EditorLayoutXmlFilePath))
            {
                var serializer = new XmlLayoutSerializer(DockingManager);
                serializer.Deserialize(EditorLayoutXmlFilePath);
            }
        }

        public void SaveLayout()
        {
            var serializer = new XmlLayoutSerializer(DockingManager);
            serializer.Serialize(EditorLayoutXmlFilePath);
        }

        private void ViewModelOnNewProjectDialogRequested(object? sender, MainViewModel.NewProjectDialogRequestedEventArgs e)
        {
            var newProjectDialogWindow = new NewProjectDialogWindow(e.ViewModel)
            {
                Owner = this
            };
            newProjectDialogWindow.ShowDialog();
        }

        private void ViewModelOnOpenFileDialogRequested(object? sender, OpenFileDialogEventArgs e)
        {
            OpenFileDialog.HandleEvent(e, this);
        }

        private void ViewModelOnCloseRequested(object? sender, EventArgs e)
        {
            Close();
        }
    }
}