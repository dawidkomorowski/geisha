using System;
using System.IO;
using Geisha.Common.Serialization;
using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSprite.Model
{
    [TestFixture]
    public class CreateSpriteServiceTests
    {
        [Test]
        public void CreateSprite_ShouldCreateSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureFile()
        {
            // Arrange
            const string json = "some json";
            var jsonSerializer = Substitute.For<IJsonSerializer>();
            var parentFolder = Substitute.For<IProjectFolder>();
            var textureFile = Substitute.For<IProjectFile>();

            textureFile.Name.Returns($"TextureForSprite{RenderingFileExtensions.Texture}");
            textureFile.ParentFolder.Returns(parentFolder);

            var createSpriteService = new CreateSpriteService(jsonSerializer);

            // Define assertions
            jsonSerializer.Serialize(Arg.Do<SpriteFileContent>(content =>
            {
                Assert.That(content.AssetId, Is.Not.EqualTo(Guid.Empty));
                // TODO Assert rest of sprite file content properties.
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
            createSpriteService.CreateSprite(textureFile);

            // Assert
            parentFolder.Received().AddFile($"TextureForSprite{RenderingFileExtensions.Sprite}", Arg.Any<Stream>());
        }
    }
}