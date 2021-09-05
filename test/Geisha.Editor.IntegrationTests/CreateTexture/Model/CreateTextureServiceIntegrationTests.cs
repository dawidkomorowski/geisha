using System;
using System.IO;
using System.Linq;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Editor.IntegrationTests.CreateTexture.Model
{
    [TestFixture]
    public class CreateTextureServiceIntegrationTests
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
        public void CreateTexture_ShouldCreateTextureAssetFileInTheSameFolderAsSourceTextureFile_GivenSourceTextureFile()
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
            var sourceTextureFile = project.Files.Single();

            var createTextureService = new CreateTextureService();

            // Act
            var textureFile = createTextureService.CreateTexture(sourceTextureFile);

            // Assert
            var textureAssetFilePath = Path.Combine(project.FolderPath, $"TestTexture{AssetFileExtension.Value}");
            Assert.That(File.Exists(textureAssetFilePath), Is.True, "Texture asset file was not created.");

            Assert.That(textureFile, Is.Not.Null);
            Assert.That(textureFile.Path, Is.EqualTo(textureAssetFilePath));

            using var fileStream = File.OpenRead(textureAssetFilePath);
            var assetData = AssetData.Load(fileStream);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));

            var textureFileContent = assetData.ReadJsonContent<TextureFileContent>();
            Assert.That(textureFileContent.TextureFilePath, Is.EqualTo("TestTexture.png"));
        }
    }
}