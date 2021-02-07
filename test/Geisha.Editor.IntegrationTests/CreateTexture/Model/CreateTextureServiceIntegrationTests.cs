using System;
using System.IO;
using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.IntegrationTests.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateTexture.Model
{
    [TestFixture]
    public class CreateTextureServiceIntegrationTests : ProjectHandlingIntegrationTestsBase
    {
        [Test]
        public void CreateTexture_ShouldCreateTextureAssetFileInTheSameFolderAsSourceTextureFile_GivenSourceTextureFile()
        {
            // Arrange
            var projectName = Path.GetRandomFileName();
            var projectLocation = GetProjectLocation();

            var project = Project.Create(projectName, projectLocation);
            var projectFilePath = project.ProjectFilePath;

            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInProject = Path.Combine(project.FolderPath, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInProject);

            project = Project.Open(projectFilePath);
            var pngProjectFile = project.Files.Single();

            var jsonSerializer = new JsonSerializer();
            var createTextureService = new CreateTextureService(jsonSerializer);

            // Act
            var textureFile = createTextureService.CreateTexture(pngProjectFile);

            // Assert
            var textureFilePath = Path.Combine(project.FolderPath, $"TestTexture{RenderingFileExtensions.Texture}");
            Assert.That(File.Exists(textureFilePath), Is.True, "Texture file was not created.");

            Assert.That(textureFile, Is.Not.Null);
            Assert.That(textureFile.Path, Is.EqualTo(textureFilePath));

            var json = File.ReadAllText(textureFilePath);
            var textureFileContent = jsonSerializer.Deserialize<TextureFileContent>(json);
            Assert.That(textureFileContent.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(textureFileContent.TextureFilePath, Is.EqualTo("TestTexture.png"));
        }
    }
}