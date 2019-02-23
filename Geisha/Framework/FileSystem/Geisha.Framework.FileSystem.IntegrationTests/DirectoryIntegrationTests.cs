using System.IO;
using System.Linq;
using Geisha.Common.TestUtils;
using NUnit.Framework;

namespace Geisha.Framework.FileSystem.IntegrationTests
{
    [TestFixture]
    public class DirectoryIntegrationTests
    {
        private IFileSystem _fileSystem;
        private string _rootDirectoryName;
        private string _rootDirectoryPath;
        private string _subdirectory1Path;
        private string _subdirectory2Path;
        private string _file1Path;
        private string _file2Path;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FileSystem();

            _rootDirectoryName = Utils.Random.GetString();
            _rootDirectoryPath = Utils.GetPathUnderTestDirectory(_rootDirectoryName);
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
            File.Delete(_file2Path);
            File.Delete(_file1Path);
            Directory.Delete(_subdirectory2Path);
            Directory.Delete(_subdirectory1Path);
            Directory.Delete(_rootDirectoryPath);
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
            var subdirectories = directory.Directories;

            // Assert
            Assert.That(subdirectories.Count(), Is.EqualTo(2));
            Assert.That(subdirectories.Select(d => d.Path), Contains.Item(_subdirectory1Path));
            Assert.That(subdirectories.Select(d => d.Path), Contains.Item(_subdirectory2Path));
        }

        [Test]
        public void Files_ShouldReturnFilesInSpecifiedDirectory()
        {
            // Arrange
            var directory = _fileSystem.GetDirectory(_rootDirectoryPath);

            // Act
            var files = directory.Files;

            // Assert
            Assert.That(files.Count(), Is.EqualTo(2));
            Assert.That(files.Select(d => d.Path), Contains.Item(_file1Path));
            Assert.That(files.Select(d => d.Path), Contains.Item(_file2Path));
        }
    }
}