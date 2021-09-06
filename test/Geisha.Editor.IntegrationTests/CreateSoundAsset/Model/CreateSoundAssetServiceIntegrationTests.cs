using System;
using System.IO;
using System.Linq;
using Geisha.Editor.CreateSoundAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
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
        public void CreateSoundAsset_ShouldCreateSoundAssetFileInTheSameFolderAsSoundFile_GivenSoundFile()
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
            var sourceSoundFile = project.Files.Single();

            var createSoundAssetService = new CreateSoundAssetService();

            // Act
            createSoundAssetService.CreateSoundAsset(sourceSoundFile);

            // Assert
            var soundAssetFilePath = Path.Combine(project.FolderPath, AssetFileUtils.AppendExtension("TestSound"));
            Assert.That(File.Exists(soundAssetFilePath), Is.True, "Sound asset file was not created.");

            using var fileStream = File.OpenRead(soundAssetFilePath);
            var assetData = AssetData.Load(fileStream);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(AudioAssetTypes.Sound));

            var soundAssetContent = assetData.ReadJsonContent<SoundAssetContent>();
            Assert.That(soundAssetContent.SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }
    }
}