using System.IO;
using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.Common.TestUtils;
using Geisha.Editor.ProjectHandling.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectTests
    {
        private const string TestDirectory = "ProjectTestsDirectory";
        private static string GetProjectLocation() => Path.Combine(Utils.GetPathUnderTestDirectory(TestDirectory));

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(Utils.GetPathUnderTestDirectory(TestDirectory));
        }

        [TearDown]
        public void TearDown()
        {
            DirectoryRemover.RemoveDirectoryRecursively(Utils.GetPathUnderTestDirectory(TestDirectory));
        }

        [Test]
        public void Create_ShouldCreateProjectDirectoryWithProjectFileInsideAndReturnNewProjectInstance()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();

            // Act
            var project = Project.Create(projectName, projectLocation);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.Name, Is.EqualTo(projectName));
            var expectedProjectFilePath = Path.Combine(projectLocation, projectName, $@"{projectName}{ProjectHandlingConstants.ProjectFileExtension}");
            Assert.That(project.FilePath, Is.EqualTo(expectedProjectFilePath));
            Assert.That(project.DirectoryPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
            Assert.That(File.Exists(expectedProjectFilePath), Is.True, "Project file was not created.");
        }


        [Test]
        public void Open_ShouldOpenExistingEmptyProject()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var existingProjectFilePath = Project.Create(projectName, projectLocation).FilePath;

            // Act
            var project = Project.Open(existingProjectFilePath);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.Name, Is.EqualTo(projectName));
            Assert.That(project.FilePath, Is.EqualTo(existingProjectFilePath));
            Assert.That(project.DirectoryPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
        }

        [Test]
        public void Open_ShouldOpenExistingProjectWithFolders()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var existingProject = Project.Create(projectName, projectLocation);

            existingProject.AddFolder("Folder 1");
            existingProject.AddFolder("Folder 2");
            existingProject.AddFolder("Folder 3");

            // Act
            var project = Project.Open(existingProject.FilePath);

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(3));
            Assert.That(project.Folders.Select(f => f.Name), Is.EquivalentTo(new[] {"Folder 1", "Folder 2", "Folder 3"}));
            Assert.That(project.Folders.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(project.DirectoryPath, "Folder 1"),
                Path.Combine(project.DirectoryPath, "Folder 2"),
                Path.Combine(project.DirectoryPath, "Folder 3")
            }));
        }

        [Test]
        public void Open_ShouldOpenExistingProjectWithFiles()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var existingProject = Project.Create(projectName, projectLocation);

            File.WriteAllText(Path.Combine(existingProject.DirectoryPath, "File 1"), string.Empty);
            File.WriteAllText(Path.Combine(existingProject.DirectoryPath, "File 2"), string.Empty);
            File.WriteAllText(Path.Combine(existingProject.DirectoryPath, "File 3"), string.Empty);

            // Act
            var project = Project.Open(existingProject.FilePath);

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(3));
            Assert.That(project.Files.Select(f => f.Name), Is.EquivalentTo(new[] {"File 1", "File 2", "File 3"}));
            Assert.That(project.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(project.DirectoryPath, "File 1"),
                Path.Combine(project.DirectoryPath, "File 2"),
                Path.Combine(project.DirectoryPath, "File 3")
            }));
        }

        [Test]
        public void AddFolder_ShouldCreateNewFolderInProjectAndNotifyUsingEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var project = Project.Create(projectName, projectLocation);

            object eventSender = null;
            ProjectFolderAddedEventArgs eventArgs = null;
            project.FolderAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            project.AddFolder("New folder");

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(1));
            var folder = project.Folders.Single();
            Assert.That(folder.Name, Is.EqualTo("New folder"));
            Assert.That(folder.Path, Is.EqualTo(Path.Combine(project.DirectoryPath, "New folder")));
            Assert.That(Directory.Exists(folder.Path), Is.True);
            Assert.That(eventSender, Is.EqualTo(project));
            Assert.That(eventArgs.Folder, Is.EqualTo(folder));
        }
    }
}