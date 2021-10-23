using System.IO;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectServiceIntegrationTests
    {
        private TemporaryDirectory _temporaryDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            _temporaryDirectory = new TemporaryDirectory();
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void NewProjectServiceDoesNotHaveOpenProject()
        {
            // Arrange
            // Act
            var projectService = CreateProjectService();

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.False);
        }

        [Test]
        public void CurrentProject_ThrowsException_WhenProjectIsNotOpen()
        {
            // Arrange
            var projectService = CreateProjectService();

            // Assume
            Assume.That(projectService.ProjectIsOpen, Is.False);

            // Act
            // Assert
            Assert.That(() => { _ = projectService.CurrentProject; }, Throws.TypeOf<ProjectNotOpenException>());
        }

        [Test]
        public void CreateNewProject_ShouldCreateNewProjectAndOpenItAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var projectService = CreateProjectService();

            object? eventSender = null;
            projectService.CurrentProjectChanged += (sender, args) => eventSender = sender;

            // Act
            projectService.CreateNewProject(projectName, projectLocation);

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.True);
            var project = projectService.CurrentProject;
            Assert.That(project.ProjectName, Is.EqualTo(projectName));
            Assert.That(project.FolderPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
            Assert.That(Directory.Exists(project.FolderPath), Is.True);
            Assert.That(File.Exists(project.ProjectFilePath), Is.True);
            Assert.That(eventSender, Is.EqualTo(projectService), $"{nameof(ProjectService.CurrentProjectChanged)} event has not occured.");
        }

        [Test]
        public void OpenProject_ShouldOpenExistingProjectAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var projectService = CreateProjectService();

            object? eventSender = null;
            projectService.CurrentProjectChanged += (sender, args) => eventSender = sender;

            var existingProject = Project.Create(projectName, projectLocation);

            // Act
            projectService.OpenProject(existingProject.ProjectFilePath);

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.True);
            var project = projectService.CurrentProject;
            Assert.That(project.ProjectName, Is.EqualTo(projectName));
            Assert.That(project.FolderPath, Is.EqualTo(existingProject.FolderPath));
            Assert.That(eventSender, Is.EqualTo(projectService), $"{nameof(ProjectService.CurrentProjectChanged)} event has not occured.");
        }

        [Test]
        public void OpenProject_ShouldOpenAnotherProjectAndNotifyWithEvent_WhenOneProjectIsAlreadyOpen()
        {
            // Arrange
            var projectName1 = Path.GetRandomFileName();
            var projectName2 = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var projectService = CreateProjectService();

            var existingProject1 = Project.Create(projectName1, projectLocation);
            var existingProject2 = Project.Create(projectName2, projectLocation);

            projectService.OpenProject(existingProject1.ProjectFilePath);

            object? eventSender = null;
            projectService.CurrentProjectChanged += (sender, args) => eventSender = sender;

            // Assume
            Assume.That(projectService.ProjectIsOpen, Is.True);
            Assume.That(projectService.CurrentProject.ProjectName, Is.EqualTo(projectName1));

            // Act
            projectService.OpenProject(existingProject2.ProjectFilePath);

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.True);
            var project = projectService.CurrentProject;
            Assert.That(project.ProjectName, Is.EqualTo(projectName2));
            Assert.That(project.FolderPath, Is.EqualTo(existingProject2.FolderPath));
            Assert.That(eventSender, Is.EqualTo(projectService), $"{nameof(ProjectService.CurrentProjectChanged)} event has not occured.");
        }

        [Test]
        public void CloseProject_ShouldCloseCurrentlyOpenProjectAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var projectService = CreateProjectService();

            var existingProject = Project.Create(projectName, projectLocation);
            projectService.OpenProject(existingProject.ProjectFilePath);

            object? eventSender = null;
            projectService.CurrentProjectChanged += (sender, args) => eventSender = sender;

            // Assume
            Assume.That(projectService.ProjectIsOpen, Is.True);
            Assume.That(projectService.CurrentProject.ProjectName, Is.EqualTo(projectName));

            // Act
            projectService.CloseProject();

            // Assert
            Assert.That(projectService.ProjectIsOpen, Is.False);
            Assert.That(eventSender, Is.EqualTo(projectService), $"{nameof(ProjectService.CurrentProjectChanged)} event has not occured.");
        }

        private static ProjectService CreateProjectService()
        {
            return new ProjectService();
        }
    }
}