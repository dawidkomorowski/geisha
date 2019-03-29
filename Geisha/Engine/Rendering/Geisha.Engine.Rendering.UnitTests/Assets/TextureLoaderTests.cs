using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Rendering.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Assets
{
    [TestFixture]
    public class TextureLoaderTests
    {
        [Test]
        public void Load_ShouldLoadTextureFromFile()
        {
            // Arrange
            const string textureFilePath = @"some_directory\texture_file_path";
            const string actualTextureFilePath = @"some_directory\actual_texture_file_path";
            const string textureFileJson = "texture file json";
            var stream = Substitute.For<Stream>();
            var texture = Substitute.For<ITexture>();

            var textureFile = Substitute.For<IFile>();
            textureFile.ReadAllText().Returns(textureFileJson);
            var actualTextureFile = Substitute.For<IFile>();
            actualTextureFile.OpenRead().Returns(stream);
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFile(textureFilePath).Returns(textureFile);
            fileSystem.GetFile(actualTextureFilePath).Returns(actualTextureFile);
            var jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.Deserialize<TextureFileContent>(textureFileJson).Returns(new TextureFileContent
            {
                TextureFilePath = "actual_texture_file_path"
            });
            var renderer2D = Substitute.For<IRenderer2D>();
            renderer2D.CreateTexture(stream).Returns(texture);

            var textureLoader = new TextureLoader(renderer2D, fileSystem, jsonSerializer);

            // Act
            var actual = textureLoader.Load(textureFilePath);

            // Assert
            Assert.That(actual, Is.EqualTo(texture));
        }
    }
}