using System.IO;
using Geisha.Common.FileSystem;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.FileSystem
{
    [TestFixture]
    public class FileSystemIntegrationTests
    {
        private IFileSystem _fileSystem = null!;
        private TemporaryDirectory _temporaryDirectory = null!;
        private string _absoluteFilePath = null!;
        private string _relativeFilePath = null!;
        private string _absoluteDirectoryPath = null!;
        private string _relativeDirectoryPath = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new Common.FileSystem.FileSystem();
            _temporaryDirectory = new TemporaryDirectory();

            _absoluteFilePath = _temporaryDirectory.GetRandomFilePath();
            _relativeFilePath = Path.GetRelativePath(_temporaryDirectory.Path, _absoluteFilePath);
            File.WriteAllText(_absoluteFilePath, string.Empty);

            _absoluteDirectoryPath = _temporaryDirectory.GetRandomDirectoryPath();
            _relativeDirectoryPath = Path.GetRelativePath(_temporaryDirectory.Path, _absoluteDirectoryPath);
            Directory.CreateDirectory(_absoluteDirectoryPath);
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void CreateFile_ShouldCreateEmptyFile()
        {
            // Arrange
            var filePath = _temporaryDirectory.GetRandomFilePath();

            // Assume
            Assume.That(File.Exists(filePath), Is.False);

            // Act
            var file = _fileSystem.CreateFile(filePath);

            // Assert
            Assert.That(file.Path, Is.EqualTo(filePath));
            Assert.That(File.Exists(filePath), Is.True);
            Assert.That(File.ReadAllText(filePath), Is.Empty);
        }

        [Test]
        public void CreateFile_ShouldOverwriteExistingFileMakingItEmpty()
        {
            // Arrange
            var filePath = _temporaryDirectory.GetRandomFilePath();
            var fileContents = Utils.Random.GetString();
            File.WriteAllText(filePath, fileContents);

            // Assume
            Assume.That(File.ReadAllText(filePath), Is.EqualTo(fileContents));

            // Act
            var file = _fileSystem.CreateFile(filePath);

            // Assert
            Assert.That(file.ReadAllText(), Is.Empty);
            Assert.That(File.ReadAllText(filePath), Is.Empty);
        }

        [Test]
        public void CreateFile_ShouldCreateFile_Then_WriteAllText_ShouldWriteTextContentsToFile()
        {
            // Arrange
            var filePath = _temporaryDirectory.GetRandomFilePath();
            var fileContents = Utils.Random.GetString();

            // Assume
            Assume.That(File.Exists(filePath), Is.False);

            // Act
            var file = _fileSystem.CreateFile(filePath);
            file.WriteAllText(fileContents);

            // Assert
            Assert.That(File.ReadAllText(filePath), Is.EqualTo(fileContents));
        }

        [Test]
        public void GetFile_ShouldThrowFileNotFoundException_GivenPathToNotExistingFile()
        {
            // Arrange
            var notExistingFilePath = Path.GetFullPath("Not existing file");

            // Act
            // Assert
            Assert.That(() => { _fileSystem.GetFile(notExistingFilePath); },
                Throws.TypeOf<FileNotFoundException>().With.Matches<FileNotFoundException>(e => e.FileName == notExistingFilePath));
        }

        [Test]
        public void GetFile_ShouldReturnFile_GivenAbsolutePath()
        {
            // Arrange
            // Act
            var file = _fileSystem.GetFile(_absoluteFilePath);

            // Assert
            Assert.That(file.Path, Is.EqualTo(_absoluteFilePath));
        }

        [Test]
        public void GetFile_ShouldReturnFile_GivenRelativePath()
        {
            // Arrange
            Directory.SetCurrentDirectory(_temporaryDirectory.Path);

            // Act
            var file = _fileSystem.GetFile(_relativeFilePath);

            // Assert
            Assert.That(file.Path, Is.EqualTo(_absoluteFilePath));
        }

        [Test]
        public void GetDirectory_ShouldThrowDirectoryNotFoundException_GivenPathToNotExistingDirectory()
        {
            // Arrange
            var notExistingDirectoryPath = Path.GetFullPath("Not existing file");

            // Act
            // Assert
            Assert.That(() => { _fileSystem.GetDirectory(notExistingDirectoryPath); },
                Throws.TypeOf<DirectoryNotFoundException>().With.Message.Contains(notExistingDirectoryPath));
        }

        [Test]
        public void GetDirectory_ShouldReturnDirectory_GivenAbsolutePath()
        {
            // Arrange
            // Act
            var directory = _fileSystem.GetDirectory(_absoluteDirectoryPath);

            // Assert
            Assert.That(directory.Path, Is.EqualTo(_absoluteDirectoryPath));
        }

        [Test]
        public void GetDirectory_ShouldReturnDirectory_GivenRelativePath()
        {
            // Arrange
            Directory.SetCurrentDirectory(_temporaryDirectory.Path);

            // Act
            var directory = _fileSystem.GetDirectory(_relativeDirectoryPath);

            // Assert
            Assert.That(directory.Path, Is.EqualTo(_absoluteDirectoryPath));
        }
    }
}