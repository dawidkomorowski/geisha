using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateAsset.Model
{
    [TestFixture]
    public class CreateSoundAssetServiceTests
    {
        [Test]
        public void CreateSoundAsset_ShouldCreateSoundAssetWithAssetTool_And_IncludeSoundAssetFileInProjectFolder()
        {
            // Arrange
            const string soundFilePath = @"ParentFolder\SoundFile";
            const string soundAssetFilePath = @"ParentFolder\SoundAssetFile";
            var assetToolCreateSoundAsset = Substitute.For<IAssetToolCreateSoundAsset>();
            assetToolCreateSoundAsset.Create(soundFilePath).Returns(soundAssetFilePath);

            var createSoundAssetService = new CreateSoundAssetService(assetToolCreateSoundAsset);

            var parentFolder = Substitute.For<IProjectFolder>();
            var soundFile = Substitute.For<IProjectFile>();
            soundFile.Path.Returns(soundFilePath);
            soundFile.ParentFolder.Returns(parentFolder);

            // Act
            createSoundAssetService.CreateSoundAsset(soundFile);

            // Assert
            parentFolder.Received(1).IncludeFile("SoundAssetFile");
        }
    }
}