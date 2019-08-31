using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Domain;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.Core.ViewModels.MainWindow.NewProjectDialog
{
    [TestFixture]
    public class NewProjectDialogViewModelTests
    {
        private IRequestFilePathService _requestFilePathService;
        private IProjectServiceObsolete _projectService;
        private IWindow _window;

        [SetUp]
        public void SetUp()
        {
            _requestFilePathService = Substitute.For<IRequestFilePathService>();
            _projectService = Substitute.For<IProjectServiceObsolete>();
            _window = Substitute.For<IWindow>();
        }

        [Test]
        public void BrowseCommand_ShouldSetProjectLocationAsReceivedFromFilePathService()
        {
            // Arrange
            const string directoryPath = @"c:\SomeDirectory";

            _requestFilePathService.RequestDirectoryPath().Returns(directoryPath);
            var viewModel = GetViewModel();

            // Act
            viewModel.BrowseCommand.Execute(null);

            // Assert
            Assert.That(viewModel.ProjectLocation, Is.EqualTo(directoryPath));
        }

        [TestCase(null)]
        [TestCase("")]
        public void BrowseCommand_ShouldNotChangeProjectLocationWhenNullOrEmptyStringReceivedFromFilePathService(string directoryPath)
        {
            // Arrange
            _requestFilePathService.RequestDirectoryPath().Returns(directoryPath);
            var viewModel = GetViewModel();

            const string initialProjectLocation = @"c:\SomeDirectory";
            viewModel.ProjectLocation = initialProjectLocation;

            // Act
            viewModel.BrowseCommand.Execute(null);

            // Assert
            Assert.That(viewModel.ProjectLocation, Is.EqualTo(initialProjectLocation));
        }

        [TestCase(null, null, false)]
        [TestCase("", "", false)]
        [TestCase("SomeProject", null, false)]
        [TestCase("SomeProject", @"", false)]
        [TestCase(null, @"c:\SomeDirectory", false)]
        [TestCase("", @"c:\SomeDirectory", false)]
        [TestCase("SomeProject", @"c:\SomeDirectory", true)]
        public void OkCommand_CanBeExecutedWhenProjectNameAndLocationSet(string projectName, string projectLocation, bool expectedCanExecute)
        {
            // Arrange
            var viewModel = GetViewModel();

            viewModel.ProjectName = projectName;
            viewModel.ProjectLocation = projectLocation;

            // Act
            var actual = viewModel.OkCommand.CanExecute(null);

            // Assert
            Assert.That(actual, Is.EqualTo(expectedCanExecute));
        }

        [Test]
        public void OkCommand_ShouldCreateProject()
        {
            // Arrange
            var viewModel = GetViewModel();

            const string projectName = "SomeProject";
            const string projectLocation = @"c:\SomeDirectory";

            viewModel.ProjectName = projectName;
            viewModel.ProjectLocation = projectLocation;

            // Act
            viewModel.OkCommand.Execute(null);

            // Assert
            _projectService.Received(1).CreateNewProject(projectName, projectLocation);
        }

        [Test]
        public void OkCommand_ShouldCloseWindow()
        {
            // Arrange
            var viewModel = GetViewModel();
            viewModel.ProjectName = "SomeProject";
            viewModel.ProjectLocation = @"c:\SomeDirectory";

            // Act
            viewModel.OkCommand.Execute(null);

            // Assert
            _window.Received().Close();
        }

        [Test]
        public void CancelCommand_ShouldCloseWindow()
        {
            // Arrange
            var viewModel = GetViewModel();

            // Act
            viewModel.CancelCommand.Execute(null);

            // Assert
            _window.Received().Close();
        }

        private NewProjectDialogViewModel GetViewModel()
        {
            return new NewProjectDialogViewModel(_requestFilePathService, _projectService) {Window = _window};
        }
    }
}