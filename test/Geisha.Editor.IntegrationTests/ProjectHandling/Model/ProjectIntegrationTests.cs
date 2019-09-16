using System;
using System.IO;
using System.Linq;
using System.Text;
using Geisha.Editor.ProjectHandling.Model;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.ProjectHandling.Model
{
    [TestFixture]
    public class ProjectIntegrationTests : ProjectHandlingIntegrationTestsBase
    {
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


        #region Project.Open() tests

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
        public void Open_ShouldOpenExistingProjectWithNestedFolders()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
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
            var project = Project.Open(existingProject.FilePath);

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(2));

            var folder1 = project.Folders.Single(f => f.Name == "Folder 1");
            Assert.That(folder1.Path, Is.EqualTo(Path.Combine(project.DirectoryPath, "Folder 1")));
            Assert.That(folder1.Folders, Has.Count.EqualTo(2));
            var folder11 = folder1.Folders.Single(f => f.Name == "Folder 1.1");
            Assert.That(folder11.Path, Is.EqualTo(Path.Combine(folder1.Path, "Folder 1.1")));
            Assert.That(folder11.Folders, Has.Count.EqualTo(2));
            Assert.That(folder11.Folders.Select(f => f.Name), Is.EquivalentTo(new[] {"Folder 1.1.1", "Folder 1.1.2"}));
            Assert.That(folder11.Folders.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder11.Path, "Folder 1.1.1"),
                Path.Combine(folder11.Path, "Folder 1.1.2")
            }));
            var folder12 = folder1.Folders.Single(f => f.Name == "Folder 1.2");
            Assert.That(folder12.Path, Is.EqualTo(Path.Combine(folder1.Path, "Folder 1.2")));
            Assert.That(folder12.Folders, Has.Count.EqualTo(2));
            Assert.That(folder12.Folders.Select(f => f.Name), Is.EquivalentTo(new[] {"Folder 1.2.1", "Folder 1.2.2"}));
            Assert.That(folder12.Folders.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder12.Path, "Folder 1.2.1"),
                Path.Combine(folder12.Path, "Folder 1.2.2")
            }));

            var folder2 = project.Folders.Single(f => f.Name == "Folder 2");
            Assert.That(folder2.Path, Is.EqualTo(Path.Combine(project.DirectoryPath, "Folder 2")));
            Assert.That(folder2.Folders, Has.Count.EqualTo(2));
            var folder21 = folder2.Folders.Single(f => f.Name == "Folder 2.1");
            Assert.That(folder21.Path, Is.EqualTo(Path.Combine(folder2.Path, "Folder 2.1")));
            Assert.That(folder21.Folders, Has.Count.EqualTo(2));
            Assert.That(folder21.Folders.Select(f => f.Name), Is.EquivalentTo(new[] {"Folder 2.1.1", "Folder 2.1.2"}));
            Assert.That(folder21.Folders.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder21.Path, "Folder 2.1.1"),
                Path.Combine(folder21.Path, "Folder 2.1.2")
            }));
            var folder22 = folder2.Folders.Single(f => f.Name == "Folder 2.2");
            Assert.That(folder22.Path, Is.EqualTo(Path.Combine(folder2.Path, "Folder 2.2")));
            Assert.That(folder22.Folders, Has.Count.EqualTo(2));
            Assert.That(folder22.Folders.Select(f => f.Name), Is.EquivalentTo(new[] {"Folder 2.2.1", "Folder 2.2.2"}));
            Assert.That(folder22.Folders.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder22.Path, "Folder 2.2.1"),
                Path.Combine(folder22.Path, "Folder 2.2.2")
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
        public void Open_ShouldOpenExistingProjectWithNestedFiles()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();

            var existingProject = Project.Create(projectName, projectLocation);
            File.WriteAllText(Path.Combine(existingProject.DirectoryPath, "File 1"), string.Empty);
            File.WriteAllText(Path.Combine(existingProject.DirectoryPath, "File 2"), string.Empty);

            var existingFolder1 = existingProject.AddFolder("Folder 1");
            File.WriteAllText(Path.Combine(existingFolder1.Path, "File 1.1"), string.Empty);
            File.WriteAllText(Path.Combine(existingFolder1.Path, "File 1.2"), string.Empty);

            var existingFolder11 = existingFolder1.AddFolder("Folder 1.1");
            File.WriteAllText(Path.Combine(existingFolder11.Path, "File 1.1.1"), string.Empty);
            File.WriteAllText(Path.Combine(existingFolder11.Path, "File 1.1.2"), string.Empty);

            // Act
            var project = Project.Open(existingProject.FilePath);

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(2));
            Assert.That(project.Files.Select(f => f.Name), Is.EquivalentTo(new[] {"File 1", "File 2"}));
            Assert.That(project.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(project.DirectoryPath, "File 1"),
                Path.Combine(project.DirectoryPath, "File 2")
            }));

            var folder1 = project.Folders.Single();
            Assert.That(folder1.Files, Has.Count.EqualTo(2));
            Assert.That(folder1.Files.Select(f => f.Name), Is.EquivalentTo(new[] {"File 1.1", "File 1.2"}));
            Assert.That(folder1.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder1.Path, "File 1.1"),
                Path.Combine(folder1.Path, "File 1.2")
            }));

            var folder11 = folder1.Folders.Single();
            Assert.That(folder11.Files, Has.Count.EqualTo(2));
            Assert.That(folder11.Files.Select(f => f.Name), Is.EquivalentTo(new[] {"File 1.1.1", "File 1.1.2"}));
            Assert.That(folder11.Files.Select(f => f.Path), Is.EquivalentTo(new[]
            {
                Path.Combine(folder11.Path, "File 1.1.1"),
                Path.Combine(folder11.Path, "File 1.1.2")
            }));
        }

        #endregion

        [Test]
        public void AddFolder_ShouldCreateNewFolderInProjectAndNotifyWithEvent()
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
            var newFolder = project.AddFolder("New folder");

            // Assert
            Assert.That(project.Folders, Has.Count.EqualTo(1));
            Assert.That(newFolder, Is.EqualTo(project.Folders.Single()));
            Assert.That(newFolder.Name, Is.EqualTo("New folder"));
            Assert.That(newFolder.Path, Is.EqualTo(Path.Combine(project.DirectoryPath, "New folder")));
            Assert.That(Directory.Exists(newFolder.Path), Is.True, "Folder was not created.");
            Assert.That(eventSender, Is.EqualTo(project));
            Assert.That(eventArgs.Folder, Is.EqualTo(newFolder));
        }

        [Test]
        public void AddFile_ShouldCreateNewFileInProjectAndNotifyWithEvent()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();
            var project = Project.Create(projectName, projectLocation);
            var fileContent = Guid.NewGuid().ToString();

            object eventSender = null;
            ProjectFileAddedEventArgs eventArgs = null;
            project.FileAdded += (sender, args) =>
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

                newFile = project.AddFile("SomeFile.txt", memoryStream);
            }

            // Assert
            Assert.That(project.Files, Has.Count.EqualTo(1));
            Assert.That(newFile, Is.EqualTo(project.Files.Single()));
            Assert.That(newFile.Name, Is.EqualTo("SomeFile.txt"));
            Assert.That(newFile.Path, Is.EqualTo(Path.Combine(project.DirectoryPath, "SomeFile.txt")));
            Assert.That(File.Exists(newFile.Path), Is.True, "File was not created.");
            Assert.That(File.ReadAllText(newFile.Path), Is.EqualTo(fileContent));
            Assert.That(eventSender, Is.EqualTo(project));
            Assert.That(eventArgs.File, Is.EqualTo(newFile));
        }
    }
}