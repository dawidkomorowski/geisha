using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Geisha.Editor.CreateSoundAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateSoundAsset.Model
{
    [TestFixture]
    public class CreateSoundAssetServiceIntegrationTests
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
        public void CreateSound_ShouldCreateSoundAssetFileInTheSameFolderAsSoundFile_GivenSoundFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInProject = Path.Combine(project.FolderPath, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInProject);

            project = Project.Open(projectFilePath);
            var soundProjectFile = project.Files.Single();

            var createSoundAssetService = new CreateSoundAssetService();

            // Act
            createSoundAssetService.CreateSoundAsset(soundProjectFile);

            // Assert
            var soundFilePath = Path.Combine(project.FolderPath, $"TestSound{AudioFileExtensions.Sound}");
            Assert.That(File.Exists(soundFilePath), Is.True, "Sound file was not created.");

            var json = File.ReadAllText(soundFilePath);
            var soundFileContent = JsonSerializer.Deserialize<SoundFileContent>(json);
            Assert.That(soundFileContent, Is.Not.Null);
            Assert.That(soundFileContent!.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(soundFileContent.SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }
    }
}