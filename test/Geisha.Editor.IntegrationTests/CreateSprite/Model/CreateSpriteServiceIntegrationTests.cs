using System;
using System.IO;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Common.TestUtils;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.IntegrationTests.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.IntegrationTestsData;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateSprite.Model
{
    [TestFixture]
    public class CreateSpriteServiceIntegrationTests : ProjectHandlingIntegrationTestsBase
    {
        [Test]
        public void CreateSprite_ShouldCreateSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInProject = Path.Combine(project.FolderPath, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInProject);

            var textureFilePathToCopy = Utils.GetPathUnderTestDirectory($@"Assets\TestTexture{RenderingFileExtensions.Texture}");
            var textureFilePathInProject = Path.Combine(project.FolderPath, $"TestTexture{RenderingFileExtensions.Texture}");
            File.Copy(textureFilePathToCopy, textureFilePathInProject);

            project = Project.Open(projectFilePath);
            var textureProjectFile = project.Files.Single(f => f.Extension == RenderingFileExtensions.Texture);

            var jsonSerializer = new JsonSerializer();
            var createSpriteService = new CreateSpriteService(jsonSerializer);

            // Act
            createSpriteService.CreateSprite(textureProjectFile);

            // Assert
            var spriteFilePath = Path.Combine(project.FolderPath, $"TestTexture{RenderingFileExtensions.Sprite}");
            Assert.That(File.Exists(spriteFilePath));

            var json = File.ReadAllText(spriteFilePath);
            var spriteFileContent = jsonSerializer.Deserialize<SpriteFileContent>(json);
            Assert.That(spriteFileContent.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(spriteFileContent.TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
            Assert.That(spriteFileContent.SourceUV.X, Is.Zero);
            Assert.That(spriteFileContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteFileContent.SourceDimension.X, Is.EqualTo(10));
            Assert.That(spriteFileContent.SourceDimension.Y, Is.EqualTo(10));
            Assert.That(spriteFileContent.SourceAnchor.X, Is.EqualTo(5));
            Assert.That(spriteFileContent.SourceAnchor.Y, Is.EqualTo(5));
            Assert.That(spriteFileContent.PixelsPerUnit, Is.EqualTo(1));
        }
    }
}