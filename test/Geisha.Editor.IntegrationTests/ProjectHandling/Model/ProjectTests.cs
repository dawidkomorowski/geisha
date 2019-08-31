using System.IO;
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
        private string _testDirectory = "ProjectTestsDirectory";

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(Utils.GetPathUnderTestDirectory(_testDirectory));
        }

        [TearDown]
        public void TearDown()
        {
            DirectoryRemover.RemoveDirectoryRecursively(Utils.GetPathUnderTestDirectory(_testDirectory));
        }

        [Test]
        public void Create_ShouldCreateProjectDirectoryWithProjectFileInsideAndReturnNewProjectInstance()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = Path.Combine(Utils.GetPathUnderTestDirectory(_testDirectory));

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
            var projectLocation = Path.Combine(Utils.GetPathUnderTestDirectory(_testDirectory));
            var existingProjectFilePath = Project.Create(projectName, projectLocation).FilePath;

            // Act
            var project = Project.Open(existingProjectFilePath);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.Name, Is.EqualTo(projectName));
            Assert.That(project.FilePath, Is.EqualTo(existingProjectFilePath));
            Assert.That(project.DirectoryPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
        }
    }
}