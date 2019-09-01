using System.IO;
using System.Linq;
using Geisha.Editor.ProjectHandling.Model;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectFolderTests : ProjectTestsBase
    {
        [Test]
        public void AddFolder_ShouldCreateNewFolderInProjectFolderAndNotifyUsingEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var project = Project.Create(projectName, projectLocation);
            var folder = project.AddFolder("FolderUnderTest");

            object eventSender = null;
            ProjectFolderAddedEventArgs eventArgs = null;
            folder.FolderAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            var newFolder = folder.AddFolder("New folder");

            // Assert
            Assert.That(folder.Folders, Has.Count.EqualTo(1));
            Assert.That(newFolder, Is.EqualTo(folder.Folders.Single()));
            Assert.That(newFolder.Name, Is.EqualTo("New folder"));
            Assert.That(newFolder.Path, Is.EqualTo(Path.Combine(folder.Path, "New folder")));
            Assert.That(Directory.Exists(newFolder.Path), Is.True);
            Assert.That(eventSender, Is.EqualTo(folder));
            Assert.That(eventArgs.Folder, Is.EqualTo(newFolder));
        }
    }
}