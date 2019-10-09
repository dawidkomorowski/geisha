using System;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add.NewFolder;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add.NewFolder
{
    [TestFixture]
    public class AddNewFolderDialogViewModelTests
    {
        private IProject _project;
        private IProjectFolder _projectFolder;

        [SetUp]
        public void SetUp()
        {
            _project = Substitute.For<IProject>();
            _projectFolder = Substitute.For<IProjectFolder>();
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("New folder", true)]
        public void OkCommand_CannotBeExecuted_WhenFolderNameIsNullOrEmpty(string folderName, bool canExecute)
        {
            // Arrange
            var viewModel = new AddNewFolderDialogViewModel(_project);

            // Act
            viewModel.FolderName = folderName;

            // Assert
            Assert.That(viewModel.OkCommand.CanExecute(null), Is.EqualTo(canExecute));
        }

        [Test]
        public void OkCommand_ShouldAddFolderToProjectAndRaiseCloseRequestedEvent()
        {
            // Arrange
            var viewModel = new AddNewFolderDialogViewModel(_project);

            object eventSender = null;
            EventArgs eventArgs = null;
            viewModel.CloseRequested += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            viewModel.FolderName = "New folder";

            // Act
            viewModel.OkCommand.Execute(null);

            // Assert
            _project.Received(1).AddFolder("New folder");
            Assert.That(eventSender, Is.EqualTo(viewModel));
            Assert.That(eventArgs, Is.EqualTo(EventArgs.Empty));
        }

        [Test]
        public void OkCommand_ShouldAddFolderToFolderAndRaiseCloseRequestedEvent()
        {
            // Arrange
            var viewModel = new AddNewFolderDialogViewModel(_projectFolder);

            object eventSender = null;
            EventArgs eventArgs = null;
            viewModel.CloseRequested += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            viewModel.FolderName = "New folder";

            // Act
            viewModel.OkCommand.Execute(null);

            // Assert
            _projectFolder.Received(1).AddFolder("New folder");
            Assert.That(eventSender, Is.EqualTo(viewModel));
            Assert.That(eventArgs, Is.EqualTo(EventArgs.Empty));
        }

        [Test]
        public void CancelCommand_ShouldRaiseCloseRequestedEvent()
        {
            // Arrange
            var viewModel = new AddNewFolderDialogViewModel(_project);

            object eventSender = null;
            EventArgs eventArgs = null;
            viewModel.CloseRequested += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            viewModel.CancelCommand.Execute(null);

            // Assert
            Assert.That(eventSender, Is.EqualTo(viewModel));
            Assert.That(eventArgs, Is.EqualTo(EventArgs.Empty));
        }
    }
}