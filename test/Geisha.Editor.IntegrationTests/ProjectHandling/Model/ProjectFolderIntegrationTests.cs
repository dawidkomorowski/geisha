using System;
using System.IO;
using System.Linq;
using System.Text;
using Geisha.Editor.ProjectHandling.Model;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectFolderIntegrationTests : ProjectHandlingIntegrationTestsBase
    {
        [Test]
        public void AddFolder_ShouldCreateNewFolderInProjectFolderAndNotifyWithEvent()
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
            Assert.That(newFolder.FolderName, Is.EqualTo("New folder"));
            Assert.That(newFolder.FolderPath, Is.EqualTo(Path.Combine(folder.FolderPath, "New folder")));
            Assert.That(Directory.Exists(newFolder.FolderPath), Is.True);
            Assert.That(eventSender, Is.EqualTo(folder));
            Assert.That(eventArgs.Folder, Is.EqualTo(newFolder));
        }

        [Test]
        public void AddFile_ShouldCreateNewFileInProjectAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var project = Project.Create(projectName, projectLocation);
            var folder = project.AddFolder("FolderUnderTest");
            var fileContent = Guid.NewGuid().ToString();

            object eventSender = null;
            ProjectFileAddedEventArgs eventArgs = null;
            folder.FileAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            IProjectFile newFile;
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 512, true))
                {
                    streamWriter.Write(fileContent);
                }

                memoryStream.Position = 0;

                newFile = folder.AddFile("SomeFile.txt", memoryStream);
            }

            // Assert
            Assert.That(folder.Files, Has.Count.EqualTo(1));
            Assert.That(newFile, Is.EqualTo(folder.Files.Single()));
            Assert.That(newFile.Name, Is.EqualTo("SomeFile.txt"));
            Assert.That(newFile.Path, Is.EqualTo(Path.Combine(folder.FolderPath, "SomeFile.txt")));
            Assert.That(File.Exists(newFile.Path), Is.True, "File was not created.");
            Assert.That(File.ReadAllText(newFile.Path), Is.EqualTo(fileContent));
            Assert.That(eventSender, Is.EqualTo(folder));
            Assert.That(eventArgs.File, Is.EqualTo(newFile));
        }
    }
}