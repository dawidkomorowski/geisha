using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Geisha.Common;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectIntegrationTests
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
        public void Create_ShouldCreateFolderForProjectWithProjectFileInsideAndReturnNewProjectInstance()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

            // Act
            var project = Project.Create(projectName, projectLocation);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.ProjectName, Is.EqualTo(projectName));
            var expectedProjectFilePath = Path.Combine(projectLocation, projectName, $@"{projectName}{ProjectHandlingConstants.ProjectFileExtension}");
            Assert.That(project.ProjectFilePath, Is.EqualTo(expectedProjectFilePath));
            Assert.That(project.FolderPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
            Assert.That(File.Exists(expectedProjectFilePath), Is.True, "Project file was not created.");
        }


        #region Project.Open() tests

        [Test]
        public void Open_ShouldOpenExistingEmptyProject()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var existingProjectFilePath = Project.Create(projectName, projectLocation).ProjectFilePath;

            // Act
            var project = Project.Open(existingProjectFilePath);

            // Assert
            Assert.That(project, Is.Not.Null);
            Assert.That(project.ProjectName, Is.EqualTo(projectName));
            Assert.That(project.ProjectFilePath, Is.EqualTo(existingProjectFilePath));
            Assert.That(project.FolderPath, Is.EqualTo(Path.Combine(projectLocation, projectName)));
            Assert.That(project.Files, Has.Count.Zero);
            Assert.That(project.Folders, Has.Count.Zero);
        }

        [Test]
        public void Open_ShouldOpenExistingProjectWithFolders()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var existingProject = Project.Create(projectName, projectLocation);

            existingProject.AddFolder("Folder 1");
            existingProject.AddFolder("Folder 2");
            existingProject.AddFolder("Folder 3");

            // Act
            var project = Project.Open(existingProject.ProjectFilePath);

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(3));
            Assert.That(project.Folders.Select(f => f.FolderName), Is.EquivalentTo(new[] { "Folder 1", "Folder 2", "Folder 3" }));
            Assert.That(project.Folders.Select(f => f.FolderPath), Is.EquivalentTo(new[]
            {
                Path.Combine(project.FolderPath, "Folder 1"),
                Path.Combine(project.FolderPath, "Folder 2"),
                Path.Combine(project.FolderPath, "Folder 3")
            }));
        }

        [Test]
        public void Open_ShouldOpenExistingProjectWithNestedFolders()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var existingProject = Project.Create(projectName, projectLocation);

            var existingFolder1 = existingProject.AddFolder("Folder 1");
            var existingFolder11 = existingFolder1.AddFolder("Folder 1.1");
            existingFolder11.AddFolder("Folder 1.1.1");
            existingFolder11.AddFolder("Folder 1.1.2");
            var existingFolder12 = existingFolder1.AddFolder("Folder 1.2");
            existingFolder12.AddFolder("Folder 1.2.1");
            existingFolder12.AddFolder("Folder 1.2.2");
            var existingFolder2 = existingProject.AddFolder("Folder 2");
            var existingFolder21 = existingFolder2.AddFolder("Folder 2.1");
            existingFolder21.AddFolder("Folder 2.1.1");
            existingFolder21.AddFolder("Folder 2.1.2");
            var existingFolder22 = existingFolder2.AddFolder("Folder 2.2");
            existingFolder22.AddFolder("Folder 2.2.1");
            existingFolder22.AddFolder("Folder 2.2.2");

            // Act
            var project = Project.Open(existingProject.ProjectFilePath);

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(2));

            var folder1 = project.Folders.Single(f => f.FolderName == "Folder 1");
            Assert.That(folder1.FolderPath, Is.EqualTo(Path.Combine(project.FolderPath, "Folder 1")));
            Assert.That(folder1.Folders, Has.Count.EqualTo(2));
            var folder11 = folder1.Folders.Single(f => f.FolderName == "Folder 1.1");
            Assert.That(folder11.FolderPath, Is.EqualTo(Path.Combine(folder1.FolderPath, "Folder 1.1")));
            Assert.That(folder11.Folders, Has.Count.EqualTo(2));
            Assert.That(folder11.Folders.Select(f => f.FolderName), Is.EquivalentTo(new[] { "Folder 1.1.1", "Folder 1.1.2" }));
            Assert.That(folder11.Folders.Select(f => f.FolderPath), Is.EquivalentTo(new[]
            {
                Path.Combine(folder11.FolderPath, "Folder 1.1.1"),
                Path.Combine(folder11.FolderPath, "Folder 1.1.2")
            }));
            var folder12 = folder1.Folders.Single(f => f.FolderName == "Folder 1.2");
            Assert.That(folder12.FolderPath, Is.EqualTo(Path.Combine(folder1.FolderPath, "Folder 1.2")));
            Assert.That(folder12.Folders, Has.Count.EqualTo(2));
            Assert.That(folder12.Folders.Select(f => f.FolderName), Is.EquivalentTo(new[] { "Folder 1.2.1", "Folder 1.2.2" }));
            Assert.That(folder12.Folders.Select(f => f.FolderPath), Is.EquivalentTo(new[]
            {
                Path.Combine(folder12.FolderPath, "Folder 1.2.1"),
                Path.Combine(folder12.FolderPath, "Folder 1.2.2")
            }));

            var folder2 = project.Folders.Single(f => f.FolderName == "Folder 2");
            Assert.That(folder2.FolderPath, Is.EqualTo(Path.Combine(project.FolderPath, "Folder 2")));
            Assert.That(folder2.Folders, Has.Count.EqualTo(2));
            var folder21 = folder2.Folders.Single(f => f.FolderName == "Folder 2.1");
            Assert.That(folder21.FolderPath, Is.EqualTo(Path.Combine(folder2.FolderPath, "Folder 2.1")));
            Assert.That(folder21.Folders, Has.Count.EqualTo(2));
            Assert.That(folder21.Folders.Select(f => f.FolderName), Is.EquivalentTo(new[] { "Folder 2.1.1", "Folder 2.1.2" }));
            Assert.That(folder21.Folders.Select(f => f.FolderPath), Is.EquivalentTo(new[]
            {
                Path.Combine(folder21.FolderPath, "Folder 2.1.1"),
                Path.Combine(folder21.FolderPath, "Folder 2.1.2")
            }));
            var folder22 = folder2.Folders.Single(f => f.FolderName == "Folder 2.2");
            Assert.That(folder22.FolderPath, Is.EqualTo(Path.Combine(folder2.FolderPath, "Folder 2.2")));
            Assert.That(folder22.Folders, Has.Count.EqualTo(2));
            Assert.That(folder22.Folders.Select(f => f.FolderName), Is.EquivalentTo(new[] { "Folder 2.2.1", "Folder 2.2.2" }));
            Assert.That(folder22.Folders.Select(f => f.FolderPath), Is.EquivalentTo(new[]
            {
                Path.Combine(folder22.FolderPath, "Folder 2.2.1"),
                Path.Combine(folder22.FolderPath, "Folder 2.2.2")
            }));
        }

        [Test]
        public void Open_ShouldOpenExistingProjectWithFiles()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var existingProject = Project.Create(projectName, projectLocation);

            File.WriteAllText(Path.Combine(existingProject.FolderPath, "File 1.txt"), string.Empty);
            File.WriteAllText(Path.Combine(existingProject.FolderPath, "File 2.txt"), string.Empty);
            File.WriteAllText(Path.Combine(existingProject.FolderPath, "File 3.txt"), string.Empty);

            // Act
            var project = Project.Open(existingProject.ProjectFilePath);

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(3));
            Assert.That(project.Files.Select(f => f.Name), Is.EquivalentTo(new[] { "File 1.txt", "File 2.txt", "File 3.txt" }));
            Assert.That(project.Files.Select(f => f.Extension), Is.EquivalentTo(new[] { ".txt", ".txt", ".txt" }));
            Assert.That(project.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(project.FolderPath, "File 1.txt"),
                Path.Combine(project.FolderPath, "File 2.txt"),
                Path.Combine(project.FolderPath, "File 3.txt")
            }));
            Assert.That(project.Files.ElementAt(0).ParentFolder, Is.EqualTo(project));
            Assert.That(project.Files.ElementAt(1).ParentFolder, Is.EqualTo(project));
            Assert.That(project.Files.ElementAt(2).ParentFolder, Is.EqualTo(project));
        }

        [Test]
        public void Open_ShouldOpenExistingProjectWithNestedFiles()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

            var existingProject = Project.Create(projectName, projectLocation);
            File.WriteAllText(Path.Combine(existingProject.FolderPath, "File 1.txt"), string.Empty);
            File.WriteAllText(Path.Combine(existingProject.FolderPath, "File 2.txt"), string.Empty);

            var existingFolder1 = existingProject.AddFolder("Folder 1");
            File.WriteAllText(Path.Combine(existingFolder1.FolderPath, "File 1.1.txt"), string.Empty);
            File.WriteAllText(Path.Combine(existingFolder1.FolderPath, "File 1.2.txt"), string.Empty);

            var existingFolder11 = existingFolder1.AddFolder("Folder 1.1");
            File.WriteAllText(Path.Combine(existingFolder11.FolderPath, "File 1.1.1.txt"), string.Empty);
            File.WriteAllText(Path.Combine(existingFolder11.FolderPath, "File 1.1.2.txt"), string.Empty);

            // Act
            var project = Project.Open(existingProject.ProjectFilePath);

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(2));
            Assert.That(project.Files.Select(f => f.Name), Is.EquivalentTo(new[] { "File 1.txt", "File 2.txt" }));
            Assert.That(project.Files.Select(f => f.Extension), Is.EquivalentTo(new[] { ".txt", ".txt" }));
            Assert.That(project.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(project.FolderPath, "File 1.txt"),
                Path.Combine(project.FolderPath, "File 2.txt")
            }));
            Assert.That(project.Files.ElementAt(0).ParentFolder, Is.EqualTo(project));
            Assert.That(project.Files.ElementAt(1).ParentFolder, Is.EqualTo(project));

            var folder1 = project.Folders.Single();
            Assert.That(folder1.Files, Has.Count.EqualTo(2));
            Assert.That(folder1.Files.Select(f => f.Name), Is.EquivalentTo(new[] { "File 1.1.txt", "File 1.2.txt" }));
            Assert.That(folder1.Files.Select(f => f.Extension), Is.EquivalentTo(new[] { ".txt", ".txt" }));
            Assert.That(folder1.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder1.FolderPath, "File 1.1.txt"),
                Path.Combine(folder1.FolderPath, "File 1.2.txt")
            }));
            Assert.That(folder1.Files.ElementAt(0).ParentFolder, Is.EqualTo(folder1));
            Assert.That(folder1.Files.ElementAt(1).ParentFolder, Is.EqualTo(folder1));

            var folder11 = folder1.Folders.Single();
            Assert.That(folder11.Files, Has.Count.EqualTo(2));
            Assert.That(folder11.Files.Select(f => f.Name), Is.EquivalentTo(new[] { "File 1.1.1.txt", "File 1.1.2.txt" }));
            Assert.That(folder11.Files.Select(f => f.Extension), Is.EquivalentTo(new[] { ".txt", ".txt" }));
            Assert.That(folder11.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder11.FolderPath, "File 1.1.1.txt"),
                Path.Combine(folder11.FolderPath, "File 1.1.2.txt")
            }));
            Assert.That(folder11.Files.ElementAt(0).ParentFolder, Is.EqualTo(folder11));
            Assert.That(folder11.Files.ElementAt(1).ParentFolder, Is.EqualTo(folder11));
        }

        #endregion

        [Test]
        public void AddFolder_ShouldCreateNewFolderInProjectAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);

            object? eventSender = null;
            ProjectFolderAddedEventArgs? eventArgs = null;
            project.FolderAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            var newFolder = project.AddFolder("New folder");

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(1));
            Assert.That(newFolder, Is.EqualTo(project.Folders.Single()));
            Assert.That(newFolder.FolderName, Is.EqualTo("New folder"));
            Assert.That(newFolder.FolderPath, Is.EqualTo(Path.Combine(project.FolderPath, "New folder")));
            Assert.That(Directory.Exists(newFolder.FolderPath), Is.True, "Folder was not created.");
            Assert.That(eventSender, Is.EqualTo(project));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.Folder, Is.EqualTo(newFolder));
        }

        [Test]
        public void AddFolder_ShouldThrowArgumentException_GivenFolderNameThatAlreadyExistsInProject()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);

            project.AddFolder("New folder");

            // Act
            // Assert
            Assert.That(() => project.AddFolder("New folder"), Throws.ArgumentException);
        }

        [Test]
        public void AddFile_ShouldCreateNewFileInProjectAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);
            var fileContent = Guid.NewGuid().ToString();

            object? eventSender = null;
            ProjectFileAddedEventArgs? eventArgs = null;
            project.FileAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            IProjectFile newFile;
            using (var stream = fileContent.ToStream())
            {
                newFile = project.AddFile("SomeFile.txt", stream);
            }

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(1));
            Assert.That(newFile, Is.EqualTo(project.Files.Single()));
            Assert.That(newFile.Name, Is.EqualTo("SomeFile.txt"));
            Assert.That(newFile.Extension, Is.EqualTo(".txt"));
            Assert.That(newFile.Path, Is.EqualTo(Path.Combine(project.FolderPath, "SomeFile.txt")));
            Assert.That(newFile.ParentFolder, Is.EqualTo(project));
            Assert.That(File.Exists(newFile.Path), Is.True, "File was not created.");
            Assert.That(File.ReadAllText(newFile.Path), Is.EqualTo(fileContent));
            Assert.That(eventSender, Is.EqualTo(project));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.File, Is.EqualTo(newFile));
        }

        [Test]
        public void AddFile_ShouldThrowArgumentException_GivenFileNameThatAlreadyExistsInProject()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);

            using (var stream = Guid.NewGuid().ToString().ToStream())
            {
                project.AddFile("SomeFile.txt", stream);
            }

            // Act
            // Assert
            Assert.That(() =>
            {
                using var stream = Guid.NewGuid().ToString().ToStream();
                project.AddFile("SomeFile.txt", stream);
            }, Throws.ArgumentException);
        }

        [Test]
        public void IncludeFile_ShouldIncludeExistingFileInProjectFolderAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);

            File.WriteAllText(Path.Combine(project.FolderPath, "SomeFile.txt"), Guid.NewGuid().ToString());

            object? eventSender = null;
            ProjectFileAddedEventArgs? eventArgs = null;
            project.FileAdded += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            // Act
            var newFile = project.IncludeFile("SomeFile.txt");

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(1));
            Assert.That(newFile, Is.EqualTo(project.Files.Single()));
            Assert.That(newFile.Name, Is.EqualTo("SomeFile.txt"));
            Assert.That(newFile.Extension, Is.EqualTo(".txt"));
            Assert.That(newFile.Path, Is.EqualTo(Path.Combine(project.FolderPath, "SomeFile.txt")));
            Assert.That(newFile.ParentFolder, Is.EqualTo(project));
            Assert.That(eventSender, Is.EqualTo(project));
            Debug.Assert(eventArgs != null, nameof(eventArgs) + " != null");
            Assert.That(eventArgs.File, Is.EqualTo(newFile));
        }

        [Test]
        public void IncludeFile_ShouldThrowArgumentException_GivenFileNameOfNotExistentFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);

            // Act
            // Assert
            Assert.That(() => { project.IncludeFile("SomeFile.txt"); }, Throws.ArgumentException);
        }

        [Test]
        public void IncludeFile_ShouldThrowArgumentException_GivenFileNameThatAlreadyExistsInProject()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;
            var project = Project.Create(projectName, projectLocation);

            using (var stream = Guid.NewGuid().ToString().ToStream())
            {
                project.AddFile("SomeFile.txt", stream);
            }

            // Act
            // Assert
            Assert.That(() => { project.IncludeFile("SomeFile.txt"); }, Throws.ArgumentException);
        }
    }
}