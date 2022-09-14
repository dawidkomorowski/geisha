using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectFolderIntegrationTests
    {
        private TemporaryDirectory _temporaryDirectory = null!;
        private IProjectFolder _folder = null!;

        [SetUp]
        public void SetUp()
        {
            _temporaryDirectory = new TemporaryDirectory();

            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);
            _folder = project.AddFolder("FolderUnderTest");
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void AddFolder_ShouldCreateNewFolderInProjectFolderAndNotifyWithEvent()
        {
            // Arrange
            object? eventSender = null;
            ProjectFolderAddedEventArgs? eventArgs = null;
            _folder.FolderAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            var newFolder = _folder.AddFolder("New folder");

            // Assert
            Assert.That(_folder.Folders, Has.Count.EqualTo(1));
            Assert.That(newFolder, Is.EqualTo(_folder.Folders.Single()));
            Assert.That(newFolder.FolderName, Is.EqualTo("New folder"));
            Assert.That(newFolder.FolderPath, Is.EqualTo(Path.Combine(_folder.FolderPath, "New folder")));
            Assert.That(Directory.Exists(newFolder.FolderPath), Is.True);
            Assert.That(eventSender, Is.EqualTo(_folder));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.Folder, Is.EqualTo(newFolder));
        }

        [Test]
        public void AddFolder_ShouldThrowArgumentException_GivenFolderNameThatAlreadyExistsInProjectFolder()
        {
            // Arrange
            _folder.AddFolder("New folder");

            // Act
            // Assert
            Assert.That(() => _folder.AddFolder("New folder"), Throws.ArgumentException);
        }

        [Test]
        public void AddFile_ShouldCreateNewFileInProjectFolderAndNotifyWithEvent()
        {
            // Arrange
            var fileContent = Guid.NewGuid().ToString();

            object? eventSender = null;
            ProjectFileAddedEventArgs? eventArgs = null;
            _folder.FileAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            IProjectFile newFile;
            using (var stream = fileContent.ToStream())
            {
                newFile = _folder.AddFile("SomeFile.txt", stream);
            }

            // Assert
            Assert.That(_folder.Files, Has.Count.EqualTo(1));
            Assert.That(newFile, Is.EqualTo(_folder.Files.Single()));
            Assert.That(newFile.Name, Is.EqualTo("SomeFile.txt"));
            Assert.That(newFile.Extension, Is.EqualTo(".txt"));
            Assert.That(newFile.Path, Is.EqualTo(Path.Combine(_folder.FolderPath, "SomeFile.txt")));
            Assert.That(newFile.ParentFolder, Is.EqualTo(_folder));
            Assert.That(File.Exists(newFile.Path), Is.True, "File was not created.");
            Assert.That(File.ReadAllText(newFile.Path), Is.EqualTo(fileContent));
            Assert.That(eventSender, Is.EqualTo(_folder));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.File, Is.EqualTo(newFile));
        }

        [Test]
        public void AddFile_ShouldThrowArgumentException_GivenFileNameThatAlreadyExistsInProjectFolder()
        {
            // Arrange
            using (var stream = Guid.NewGuid().ToString().ToStream())
            {
                _folder.AddFile("SomeFile.txt", stream);
            }

            // Act
            // Assert
            Assert.That(() =>
            {
                using var stream = Guid.NewGuid().ToString().ToStream();
                _folder.AddFile("SomeFile.txt", stream);
            }, Throws.ArgumentException);
        }

        [Test]
        public void IncludeFile_ShouldIncludeExistingFileInProjectFolderAndNotifyWithEvent()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_folder.FolderPath, "SomeFile.txt"), Guid.NewGuid().ToString());

            object? eventSender = null;
            ProjectFileAddedEventArgs? eventArgs = null;
            _folder.FileAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            var newFile = _folder.IncludeFile("SomeFile.txt");

            // Assert
            Assert.That(_folder.Files, Has.Count.EqualTo(1));
            Assert.That(newFile, Is.EqualTo(_folder.Files.Single()));
            Assert.That(newFile.Name, Is.EqualTo("SomeFile.txt"));
            Assert.That(newFile.Extension, Is.EqualTo(".txt"));
            Assert.That(newFile.Path, Is.EqualTo(Path.Combine(_folder.FolderPath, "SomeFile.txt")));
            Assert.That(newFile.ParentFolder, Is.EqualTo(_folder));
            Assert.That(eventSender, Is.EqualTo(_folder));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.File, Is.EqualTo(newFile));
        }

        [Test]
        public void IncludeFile_ShouldThrowArgumentException_GivenFileNameOfNotExistentFile()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => { _folder.IncludeFile("SomeFile.txt"); }, Throws.ArgumentException);
        }

        [Test]
        public void IncludeFile_ShouldThrowArgumentException_GivenFileNameThatAlreadyExistsInProjectFolder()
        {
            // Arrange
            using (var stream = Guid.NewGuid().ToString().ToStream())
            {
                _folder.AddFile("SomeFile.txt", stream);
            }

            // Act
            // Assert
            Assert.That(() => { _folder.IncludeFile("SomeFile.txt"); }, Throws.ArgumentException);
        }
    }
}