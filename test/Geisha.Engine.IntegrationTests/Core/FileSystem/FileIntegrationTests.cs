using System.IO;
using Geisha.Common.FileSystem;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Core.FileSystem
{
    [TestFixture]
    public class FileIntegrationTests
    {
        private IFileSystem _fileSystem = null!;
        private TemporaryDirectory _temporaryDirectory = null!;
        private string _fileNameWithoutExtension = null!;
        private string _fileExtensionWithoutDot = null!;
        private string _absoluteFilePath = null!;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new Common.FileSystem.FileSystem();
            _temporaryDirectory = new TemporaryDirectory();

            _fileNameWithoutExtension = Utils.Random.GetString();
            _fileExtensionWithoutDot = Utils.Random.GetString();
            _absoluteFilePath = _temporaryDirectory.GetPathUnderTemporaryDirectory($"{_fileNameWithoutExtension}.{_fileExtensionWithoutDot}");
            File.WriteAllText(_absoluteFilePath, "");
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void Name_ShouldReturnFileNameWithExtension()
        {
            // Arrange
            var file = _fileSystem.GetFile(_absoluteFilePath);

            // Act
            // Assert
            Assert.That(file.Name, Is.EqualTo($"{_fileNameWithoutExtension}.{_fileExtensionWithoutDot}"));
        }

        [Test]
        public void Extension_ShouldReturnFileExtensionPrecededWithDot()
        {
            // Arrange
            var file = _fileSystem.GetFile(_absoluteFilePath);

            // Act
            // Assert
            Assert.That(file.Extension, Is.EqualTo($".{_fileExtensionWithoutDot}"));
        }

        [Test]
        public void ReadAllText_ShouldReturnTextFileContents()
        {
            // Arrange
            var fileContents = Utils.Random.GetString();
            var file = _fileSystem.GetFile(_absoluteFilePath);
            File.WriteAllText(_absoluteFilePath, fileContents);

            // Act
            var actual = file.ReadAllText();

            // Assert
            Assert.That(actual, Is.EqualTo(fileContents));
        }

        [Test]
        public void WriteAllText_ShouldWriteTextContentsToFile()
        {
            // Arrange
            var fileContents = Utils.Random.GetString();
            var file = _fileSystem.GetFile(_absoluteFilePath);

            // Assume
            Assume.That(File.ReadAllText(_absoluteFilePath), Is.Not.EqualTo(fileContents));

            // Act
            file.WriteAllText(fileContents);

            // Assert
            Assert.That(File.ReadAllText(_absoluteFilePath), Is.EqualTo(fileContents));
        }

        [Test]
        public void WriteAllText_ShouldOverwriteExistingContents()
        {
            // Arrange
            var fileContents1 = Utils.Random.GetString();
            var fileContents2 = Utils.Random.GetString();

            var file = _fileSystem.GetFile(_absoluteFilePath);
            file.WriteAllText(fileContents1);

            // Assume
            Assume.That(File.ReadAllText(_absoluteFilePath), Is.EqualTo(fileContents1));

            // Act
            file.WriteAllText(fileContents2);

            // Assert
            Assert.That(File.ReadAllText(_absoluteFilePath), Is.EqualTo(fileContents2));
        }

        [Test]
        public void OpenRead_ShouldReturnStreamThatReadsTheFile()
        {
            // Arrange
            var fileContents = new byte[100];
            Utils.Random.NextBytes(fileContents);
            File.WriteAllBytes(_absoluteFilePath, fileContents);

            // Assume
            Assume.That(File.ReadAllBytes(_absoluteFilePath), Is.EqualTo(fileContents));

            var file = _fileSystem.GetFile(_absoluteFilePath);

            // Act
            var actual = new byte[100];
            using (var stream = file.OpenRead())
            {
                stream.Read(actual, 0, actual.Length);
            }

            // Assert
            Assert.That(actual, Is.EqualTo(fileContents));
        }
    }
}