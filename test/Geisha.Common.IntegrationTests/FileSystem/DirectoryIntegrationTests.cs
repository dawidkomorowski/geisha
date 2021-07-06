using System.IO;
using System.Linq;
using Geisha.Common.FileSystem;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.FileSystem
{
    [TestFixture]
    public class DirectoryIntegrationTests
    {
        private IFileSystem _fileSystem = null!;
        private TemporaryDirectory _temporaryDirectory = null!;
        private string _rootDirectoryName = null!;
        private string _rootDirectoryPath = null!;
        private string _subdirectory1Path = null!;
        private string _subdirectory2Path = null!;
        private string _file1Path = null!;
        private string _file2Path = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new Common.FileSystem.FileSystem();
            _temporaryDirectory = new TemporaryDirectory();

            _rootDirectoryName = Utils.Random.GetString();
            _rootDirectoryPath = _temporaryDirectory.GetPathUnderTemporaryDirectory(_rootDirectoryName);
            _subdirectory1Path = Path.Combine(_rootDirectoryPath, Utils.Random.GetString());
            _subdirectory2Path = Path.Combine(_rootDirectoryPath, Utils.Random.GetString());
            _file1Path = Path.Combine(_rootDirectoryPath, Path.GetFileName(Path.GetRandomFileName()));
            _file2Path = Path.Combine(_rootDirectoryPath, Path.GetFileName(Path.GetRandomFileName()));

            Directory.CreateDirectory(_rootDirectoryPath);
            Directory.CreateDirectory(_subdirectory1Path);
            Directory.CreateDirectory(_subdirectory2Path);
            File.WriteAllText(_file1Path, string.Empty);
            File.WriteAllText(_file2Path, string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void Name_ShouldReturnDirectoryName()
        {
            // Arrange
            var directory = _fileSystem.GetDirectory(_rootDirectoryPath);

            // Act
            // Assert
            Assert.That(directory.Name, Is.EqualTo(_rootDirectoryName));
        }

        [Test]
        public void Directories_ShouldReturnSubdirectoriesOfSpecifiedDirectory()
        {
            // Arrange
            var directory = _fileSystem.GetDirectory(_rootDirectoryPath);

            // Act
            var subdirectories = directory.Directories.ToArray();

            // Assert
            Assert.That(subdirectories.Length, Is.EqualTo(2));
            Assert.That(subdirectories.Select(d => d.Path), Contains.Item(_subdirectory1Path));
            Assert.That(subdirectories.Select(d => d.Path), Contains.Item(_subdirectory2Path));
        }

        [Test]
        public void Files_ShouldReturnFilesInSpecifiedDirectory()
        {
            // Arrange
            var directory = _fileSystem.GetDirectory(_rootDirectoryPath);

            // Act
            var files = directory.Files.ToArray();

            // Assert
            Assert.That(files.Length, Is.EqualTo(2));
            Assert.That(files.Select(d => d.Path), Contains.Item(_file1Path));
            Assert.That(files.Select(d => d.Path), Contains.Item(_file2Path));
        }
    }
}