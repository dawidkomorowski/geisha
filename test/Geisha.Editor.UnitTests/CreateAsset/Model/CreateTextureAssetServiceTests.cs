using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateAsset.Model
{
    [TestFixture]
    public class CreateTextureAssetServiceTests
    {
        [Test]
        public void CreateTextureAsset_ShouldCreateTextureAssetWithAssetTool_And_IncludeTextureAssetFileInProjectFolder()
        {
            // Arrange
            const string sourceTextureFilePath = @"ParentFolder\SourceTextureFile";
            const string textureAssetFilePath = @"ParentFolder\TextureAssetFile";
            var assetToolCreateTextureAsset = Substitute.For<IAssetToolCreateTextureAsset>();
            assetToolCreateTextureAsset.Create(sourceTextureFilePath).Returns(textureAssetFilePath);

            var createTextureAssetService = new CreateTextureAssetService(assetToolCreateTextureAsset);

            var parentFolder = Substitute.For<IProjectFolder>();
            var sourceTextureFile = Substitute.For<IProjectFile>();
            sourceTextureFile.Path.Returns(sourceTextureFilePath);
            sourceTextureFile.ParentFolder.Returns(parentFolder);

            // Act
            createTextureAssetService.CreateTextureAsset(sourceTextureFile);

            // Assert
            parentFolder.Received(1).IncludeFile("TextureAssetFile");
        }
    }
}