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
            var textureProjectFile = project.Files.Single(f => Path.GetExtension(f.Name) == RenderingFileExtensions.Texture);

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
            // TODO Assert rest of sprite file content properties.
        }
    }
}