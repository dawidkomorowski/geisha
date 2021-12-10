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
            const string textureFilePath = @"ParentFolder\TextureFile";
            const string textureAssetFilePath = @"ParentFolder\TextureAssetFile";
            var assetToolCreateTextureAsset = Substitute.For<IAssetToolCreateTextureAsset>();
            assetToolCreateTextureAsset.Create(textureFilePath).Returns(textureAssetFilePath);

            var createTextureAssetService = new CreateTextureAssetService(assetToolCreateTextureAsset);

            var parentFolder = Substitute.For<IProjectFolder>();
            var textureFile = Substitute.For<IProjectFile>();
            textureFile.Path.Returns(textureFilePath);
            textureFile.ParentFolder.Returns(parentFolder);

            // Act
            createTextureAssetService.CreateTextureAsset(textureFile);

            // Assert
            parentFolder.Received(1).IncludeFile("TextureAssetFile");
        }
    }
}