using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateAsset.Model
{
    [TestFixture]
    public class CreateSpriteAssetServiceTests
    {
        [Test]
        public void CreateSpriteAsset_ShouldCreateSpriteAssetWithAssetTool_And_IncludeSpriteAssetFileInProjectFolder()
        {
            // Arrange
            const string textureAssetFilePath = @"ParentFolder\TextureAssetFile";
            const string spriteAssetFilePath = @"ParentFolder\SpriteAssetFile";
            var assetToolCreateSpriteAsset = Substitute.For<IAssetToolCreateSpriteAsset>();
            assetToolCreateSpriteAsset.Create(textureAssetFilePath).Returns((new[] { spriteAssetFilePath }, null));

            var createTextureAssetService = new CreateSpriteAssetService(assetToolCreateSpriteAsset);

            var parentFolder = Substitute.For<IProjectFolder>();
            var textureAssetFile = Substitute.For<IProjectFile>();
            textureAssetFile.Path.Returns(textureAssetFilePath);
            textureAssetFile.ParentFolder.Returns(parentFolder);

            // Act
            createTextureAssetService.CreateSpriteAsset(textureAssetFile);

            // Assert
            parentFolder.Received(1).IncludeFile("SpriteAssetFile");
        }

        [Test]
        public void CreateSpriteAsset_ShouldCreateTextureAsset_And_SpriteAssetWithAssetTool_And_IncludeTextureAssetFile_And_SpriteAssetFileInProjectFolder()
        {
            // Arrange
            const string textureFilePath = @"ParentFolder\TextureFile";
            const string textureAssetFilePath = @"ParentFolder\TextureAssetFile";
            const string spriteAssetFilePath = @"ParentFolder\SpriteAssetFile";
            var assetToolCreateSpriteAsset = Substitute.For<IAssetToolCreateSpriteAsset>();
            assetToolCreateSpriteAsset.Create(textureFilePath).Returns((spriteAssetFilePaths: new[] { spriteAssetFilePath }, textureAssetFilePath));

            var createTextureAssetService = new CreateSpriteAssetService(assetToolCreateSpriteAsset);

            var parentFolder = Substitute.For<IProjectFolder>();
            var textureFile = Substitute.For<IProjectFile>();
            textureFile.Path.Returns(textureFilePath);
            textureFile.ParentFolder.Returns(parentFolder);

            // Act
            createTextureAssetService.CreateSpriteAsset(textureFile);

            // Assert
            parentFolder.Received(1).IncludeFile("TextureAssetFile");
            parentFolder.Received(1).IncludeFile("SpriteAssetFile");
        }
    }
}