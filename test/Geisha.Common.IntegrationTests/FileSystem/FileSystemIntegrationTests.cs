using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Common.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.FileSystem
{
    [TestFixture]
    public class FileSystemIntegrationTests
    {
        private IFileSystem _fileSystem;
        private string _absoluteFilePath;
        private string _relativeFilePath;
        private string _absoluteDirectoryPath;
        private string _relativeDirectoryPath;

        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new Common.FileSystem.FileSystem();

            _absoluteFilePath = Utils.GetRandomFilePath();
            _relativeFilePath = Path.GetFileName(_absoluteFilePath);
            File.WriteAllText(_absoluteFilePath, string.Empty);

            _relativeDirectoryPath = Utils.Random.GetString();
            _absoluteDirectoryPath = Utils.GetPathUnderTestDirectory(_relativeDirectoryPath);
            Directory.CreateDirectory(_absoluteDirectoryPath);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_absoluteFilePath);
            if (File.Exists(_filePath)) File.Delete(_filePath);

            Directory.Delete(_absoluteDirectoryPath);
        }

        [Test]
        public void CreateFile_ShouldCreateEmptyFile()
        {
            // Arrange
            _filePath = Utils.GetRandomFilePath();

            // Assume
            Assume.That(File.Exists(_filePath), Is.False);

            // Act
            var file = _fileSystem.CreateFile(_filePath);

            // Assert
            Assert.That(file.Path, Is.EqualTo(_filePath));
            Assert.That(File.Exists(_filePath), Is.True);
            Assert.That(File.ReadAllText(_filePath), Is.Empty);
        }

        [Test]
        public void CreateFile_ShouldOverwriteExistingFileMakingItEmpty()
        {
            // Arrange
            _filePath = Utils.GetRandomFilePath();
            var fileContents = Utils.Random.GetString();
            File.WriteAllText(_filePath, fileContents);

            // Assume
            Assume.That(File.ReadAllText(_filePath), Is.EqualTo(fileContents));

            // Act
            var file = _fileSystem.CreateFile(_filePath);

            // Assert
            Assert.That(file.ReadAllText(), Is.Empty);
            Assert.That(File.ReadAllText(_filePath), Is.Empty);
        }

        [Test]
        public void CreateFile_ShouldCreateFile_Then_WriteAllText_ShouldWriteTextContentsToFile()
        {
            // Arrange
            _filePath = Utils.GetRandomFilePath();
            var fileContents = Utils.Random.GetString();

            // Assume
            Assume.That(File.Exists(_filePath), Is.False);

            // Act
            var file = _fileSystem.CreateFile(_filePath);
            file.WriteAllText(fileContents);

            // Assert
            Assert.That(File.ReadAllText(_filePath), Is.EqualTo(fileContents));
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
            Directory.SetCurrentDirectory(Utils.TestDirectory);

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
            Directory.SetCurrentDirectory(Utils.TestDirectory);

            // Act
            var directory = _fileSystem.GetDirectory(_relativeDirectoryPath);

            // Assert
            Assert.That(directory.Path, Is.EqualTo(_absoluteDirectoryPath));
        }
    }
}