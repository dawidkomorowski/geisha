using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.IntegrationTestsData;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateSprite.Model
{
    [TestFixture]
    public class CreateSpriteServiceIntegrationTests
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
        public void CreateSprite_ShouldCreateSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureMetadataFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

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

            var createTextureService = new CreateTextureService();
            var createSpriteService = new CreateSpriteService(createTextureService);

            // Act
            createSpriteService.CreateSprite(textureProjectFile);

            // Assert
            var spriteFilePath = Path.Combine(project.FolderPath, $"TestTexture{RenderingFileExtensions.Sprite}");
            Assert.That(File.Exists(spriteFilePath), Is.True, "Sprite file was not created.");

            var json = File.ReadAllText(spriteFilePath);
            var spriteFileContent = JsonSerializer.Deserialize<SpriteFileContent>(json);
            Assert.That(spriteFileContent, Is.Not.Null);
            Assert.That(spriteFileContent!.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(spriteFileContent.TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
            Assert.That(spriteFileContent.SourceUV.X, Is.Zero);
            Assert.That(spriteFileContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteFileContent.SourceDimension.X, Is.EqualTo(10));
            Assert.That(spriteFileContent.SourceDimension.Y, Is.EqualTo(10));
            Assert.That(spriteFileContent.SourceAnchor.X, Is.EqualTo(5));
            Assert.That(spriteFileContent.SourceAnchor.Y, Is.EqualTo(5));
            Assert.That(spriteFileContent.PixelsPerUnit, Is.EqualTo(1));
        }

        [Test]
        public void CreateSprite_ShouldCreateTextureAssetFileAndSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureDataFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = _temporaryDirectory.Path;

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInProject = Path.Combine(project.FolderPath, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInProject);

            project = Project.Open(projectFilePath);
            var textureProjectFile = project.Files.Single();

            var createTextureService = new CreateTextureService();
            var createSpriteService = new CreateSpriteService(createTextureService);

            // Act
            createSpriteService.CreateSprite(textureProjectFile);

            // Assert
            var textureFilePath = Path.Combine(project.FolderPath, $"TestTexture{RenderingFileExtensions.Texture}");
            Assert.That(File.Exists(textureFilePath), Is.True, "Texture file was not created.");

            var json = File.ReadAllText(textureFilePath);
            var textureFileContent = JsonSerializer.Deserialize<TextureFileContent>(json);
            Assert.That(textureFileContent, Is.Not.Null);
            Assert.That(textureFileContent!.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(textureFileContent.TextureFilePath, Is.EqualTo("TestTexture.png"));

            var spriteFilePath = Path.Combine(project.FolderPath, $"TestTexture{RenderingFileExtensions.Sprite}");
            Assert.That(File.Exists(spriteFilePath), Is.True, "Sprite file was not created.");

            json = File.ReadAllText(spriteFilePath);
            var spriteFileContent = JsonSerializer.Deserialize<SpriteFileContent>(json);
            Assert.That(spriteFileContent, Is.Not.Null);
            Assert.That(spriteFileContent!.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(spriteFileContent.TextureAssetId, Is.EqualTo(textureFileContent.AssetId));
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