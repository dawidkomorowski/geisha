using System.IO;
using Geisha.Editor.ProjectHandling.Domain;
using Geisha.Editor.ProjectHandling.Infrastructure;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.ProjectHandling.Domain
{
    [TestFixture]
    public class ProjectTests
    {
        [Test]
        public void Create_ShouldCreateProjectDirectoryWithProjectFileInsideAndReturnNewProjectInstance()
        {
            // Arrange
            const string projectName = "Project name";
            const string projectLocation = @"C:\some_location";

            // Act
            var project = Project.Create(projectName, projectLocation, null);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.Name, Is.EqualTo(projectName));
            Assert.That(project.FilePath,
                Is.EqualTo(Path.Combine(projectLocation, projectName, $@"{projectName}{ProjectHandlingConstants.ProjectFileExtension}")));
            Assert.That(project.DirectoryPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
        }
    }
}