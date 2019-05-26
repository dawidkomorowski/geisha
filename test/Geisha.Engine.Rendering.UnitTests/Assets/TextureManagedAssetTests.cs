﻿using System.IO;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Assets
{
    [TestFixture]
    public class TextureManagedAssetTests
    {
        private IRenderer2D _renderer2D;
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;
        private TextureManagedAsset _textureManagedAsset;

        private ITexture _texture;

        [SetUp]
        public void SetUp()
        {
            _renderer2D = Substitute.For<IRenderer2D>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();

            const string textureFilePath = @"some_directory\texture_file_path";
            const string actualTextureFilePath = @"some_directory\actual_texture_file_path";
            const string textureFileJson = "texture file json";
            var stream = Substitute.For<Stream>();
            _texture = Substitute.For<ITexture>();

            var textureFile = Substitute.For<IFile>();
            textureFile.ReadAllText().Returns(textureFileJson);
            var actualTextureFile = Substitute.For<IFile>();
            actualTextureFile.OpenRead().Returns(stream);
            _fileSystem.GetFile(textureFilePath).Returns(textureFile);
            _fileSystem.GetFile(actualTextureFilePath).Returns(actualTextureFile);
            _jsonSerializer.Deserialize<TextureFileContent>(textureFileJson).Returns(new TextureFileContent
            {
                TextureFilePath = "actual_texture_file_path"
            });
            _renderer2D.CreateTexture(stream).Returns(_texture);

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ITexture), textureFilePath);
            _textureManagedAsset = new TextureManagedAsset(assetInfo, _renderer2D, _fileSystem, _jsonSerializer);
        }

        [Test]
        public void Load_ShouldLoadTextureFromFile()
        {
            // Arrange
            // Act
            _textureManagedAsset.Load();
            var actual = (ITexture) _textureManagedAsset.AssetInstance;

            // Assert
            Assert.That(actual, Is.EqualTo(_texture));
        }

        [Test]
        public void Unload_ShouldDisposeTexture()
        {
            // Arrange
            _textureManagedAsset.Load();

            // Act
            _textureManagedAsset.Unload();

            // Assert
            _texture.Received().Dispose();
        }
    }
}