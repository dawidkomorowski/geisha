using System;
using System.IO;
using Geisha.Common.Serialization;
using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateTexture.Model
{
    [TestFixture]
    public class CreateTextureServiceTests
    {
        [Test]
        public void CreateTexture_ShouldCreateTextureAssetFileInTheSameFolderAsSourceTextureFile_GivenSourceTextureFile()
        {
            // Arrange
            const string json = "some json";
            var jsonSerializer = Substitute.For<IJsonSerializer>();
            var parentFolder = Substitute.For<IProjectFolder>();
            var sourceTextureFile = Substitute.For<IProjectFile>();

            sourceTextureFile.Name.Returns("Texture.png");
            sourceTextureFile.ParentFolder.Returns(parentFolder);

            var createTextureService = new CreateTextureService(jsonSerializer);

            // Define assertions
            jsonSerializer.Serialize(Arg.Do<TextureFileContent>(content =>
            {
                Assert.That(content.AssetId, Is.Not.EqualTo(Guid.Empty));
                Assert.That(content.TextureFilePath, Is.EqualTo("Texture.png"));
            })).Returns(json);

            parentFolder.AddFile(Arg.Any<string>(), Arg.Do<Stream>(stream =>
            {
                using (var streamReader = new StreamReader(stream))
                {
                    var content = streamReader.ReadToEnd();
                    Assert.That(content, Is.EqualTo(json));
                }
            })).ReturnsNull();

            // Act
            createTextureService.CreateTexture(sourceTextureFile);

            // Assert
            parentFolder.Received().AddFile($"Texture{RenderingFileExtensions.Texture}", Arg.Any<Stream>());
        }
    }
}