using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.UserInterface.NewProjectDialog
{
    [TestFixture]
    public class NewProjectDialogViewModelTests
    {
        private IProjectService _projectService;

        [SetUp]
        public void SetUp()
        {
            _projectService = Substitute.For<IProjectService>();
        }

        [Test]
        public void BrowseCommand_ShouldSetProjectLocationAsReceivedFromOpenFileDialog()
        {
            // Arrange
            const string directoryPath = @"c:\SomeDirectory";

            var viewModel = GetViewModel();
            viewModel.OpenFileDialogRequested += (sender, args) => args.Continuation(directoryPath);

            // Act
            viewModel.BrowseCommand.Execute(null);

            // Assert
            Assert.That(viewModel.ProjectLocation, Is.EqualTo(directoryPath));
        }

        [TestCase(null)]
        [TestCase("")]
        public void BrowseCommand_ShouldNotChangeProjectLocationWhenNullOrEmptyStringReceivedFromOpenFileDialog(string directoryPath)
        {
            // Arrange
            var viewModel = GetViewModel();
            viewModel.OpenFileDialogRequested += (sender, args) => args.Continuation(directoryPath);

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

            var closeRequested = false;
            viewModel.CloseRequested += (sender, args) => closeRequested = true;

            // Act
            viewModel.OkCommand.Execute(null);

            // Assert
            Assert.That(closeRequested, Is.True);
        }

        [Test]
        public void CancelCommand_ShouldCloseWindow()
        {
            // Arrange
            var viewModel = GetViewModel();

            var closeRequested = false;
            viewModel.CloseRequested += (sender, args) => closeRequested = true;

            // Act
            viewModel.CancelCommand.Execute(null);

            // Assert
            Assert.That(closeRequested, Is.True);
        }

        private NewProjectDialogViewModel GetViewModel()
        {
            return new NewProjectDialogViewModel(_projectService);
        }
    }
}