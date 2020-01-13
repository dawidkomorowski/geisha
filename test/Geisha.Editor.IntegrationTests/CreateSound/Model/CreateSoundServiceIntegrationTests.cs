using System;
using System.IO;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Common.TestUtils;
using Geisha.Editor.CreateSound.Model;
using Geisha.Editor.IntegrationTests.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateSound.Model
{
    [TestFixture]
    public class CreateSoundServiceIntegrationTests : ProjectHandlingIntegrationTestsBase
    {
        [Test]
        public void CreateSound_ShouldCreateSoundAssetFileInTheSameFolderAsSoundFile_GivenSoundFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInProject = Path.Combine(project.FolderPath, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInProject);

            project = Project.Open(projectFilePath);
            var soundProjectFile = project.Files.Single();

            var jsonSerializer = new JsonSerializer();
            var createSoundService = new CreateSoundService(jsonSerializer);

            // Act
            createSoundService.CreateSound(soundProjectFile);

            // Assert
            var soundFilePath = Path.Combine(project.FolderPath, $"TestSound{AudioFileExtensions.Sound}");
            Assert.That(File.Exists(soundFilePath), Is.True, "Sound file was not created.");

            var json = File.ReadAllText(soundFilePath);
            var soundFileContent = jsonSerializer.Deserialize<SoundFileContent>(json);
            Assert.That(soundFileContent.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(soundFileContent.SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }
    }
}