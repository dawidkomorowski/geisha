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
            const string sourceSoundFilePath = @"ParentFolder\SourceSoundFile";
            const string soundAssetFilePath = @"ParentFolder\SoundAssetFile";
            var assetToolCreateSoundAsset = Substitute.For<IAssetToolCreateSoundAsset>();
            assetToolCreateSoundAsset.Create(sourceSoundFilePath).Returns(soundAssetFilePath);

            var createSoundAssetService = new CreateSoundAssetService(assetToolCreateSoundAsset);

            var parentFolder = Substitute.For<IProjectFolder>();
            var sourceSoundFile = Substitute.For<IProjectFile>();
            sourceSoundFile.Path.Returns(sourceSoundFilePath);
            sourceSoundFile.ParentFolder.Returns(parentFolder);

            // Act
            createSoundAssetService.CreateSoundAsset(sourceSoundFile);

            // Assert
            parentFolder.Received(1).IncludeFile("SoundAssetFile");
        }
    }
}