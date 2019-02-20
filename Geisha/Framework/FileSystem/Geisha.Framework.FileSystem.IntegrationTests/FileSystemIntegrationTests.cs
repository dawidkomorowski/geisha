using System.IO;
using Geisha.Common.TestUtils;
using NUnit.Framework;

namespace Geisha.Framework.FileSystem.IntegrationTests
{
    [TestFixture]
    public class FileSystemIntegrationTests
    {
        private IFileSystem _fileSystem;
        private string _absoluteFilePath;
        private string _relativeFilePath;

        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new FileSystem();

            _absoluteFilePath = Utils.GetRandomFilePath();
            _relativeFilePath = Path.GetFileName(_absoluteFilePath);
            File.WriteAllText(_absoluteFilePath, string.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_absoluteFilePath);
            if (File.Exists(_filePath)) File.Delete(_filePath);
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
        public void GetFile_ShouldThrowArgumentException_GivenPathToNotExistingFile()
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

            var file = _fileSystem.GetFile(_relativeFilePath);

            // Act
            // Assert
            Assert.That(file.Path, Is.EqualTo(_absoluteFilePath));
        }
    }
}