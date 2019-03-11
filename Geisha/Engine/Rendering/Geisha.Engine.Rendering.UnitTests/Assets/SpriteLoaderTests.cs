﻿using System.IO;
using Geisha.Common.Math.Serialization;
using Geisha.Common.Serialization;
using Geisha.Engine.Rendering.Assets;
using Geisha.Framework.FileSystem;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Assets
{
    [TestFixture]
    public class SpriteLoaderTests
    {
        [Test]
        public void Load_ShouldReturnSpriteWithDataAsDefinedInSpriteFile()
        {
            // Arrange
            const string spriteFilePath = @"some_directory\sprite_file_path";
            const string textureFilePath = @"some_directory\source_texture_file_path";
            const string json = "serialized data";

            var spriteFileContent = new SpriteFileContent
            {
                SourceTextureFilePath = "source_texture_file_path",
                SourceUV = new SerializableVector2 {X = 123.456, Y = 234.567},
                SourceDimension = new SerializableVector2 {X = 345.456, Y = 456.567},
                SourceAnchor = new SerializableVector2 {X = 567.678, Y = 678.789},
                PixelsPerUnit = 123.456
            };

            var stream = Substitute.For<Stream>();
            var texture = Substitute.For<ITexture>();

            var spriteFile = Substitute.For<IFile>();
            spriteFile.ReadAllText().Returns(json);
            var jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.Deserialize<SpriteFileContent>(json).Returns(spriteFileContent);
            var textureFile = Substitute.For<IFile>();
            textureFile.OpenRead().Returns(stream);
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFile(spriteFilePath).Returns(spriteFile);
            fileSystem.GetFile(textureFilePath).Returns(textureFile);
            var renderer = Substitute.For<IRenderer2D>();
            renderer.CreateTexture(stream).Returns(texture);
            var spriteLoader = new SpriteLoader(fileSystem, jsonSerializer, renderer);

            // Act
            var actual = (Sprite) spriteLoader.Load(spriteFilePath);

            // Assert
            Assert.That(actual.SourceTexture, Is.EqualTo(texture));
            Assert.That(actual.SourceUV.X, Is.EqualTo(spriteFileContent.SourceUV.X));
            Assert.That(actual.SourceUV.Y, Is.EqualTo(spriteFileContent.SourceUV.Y));
            Assert.That(actual.SourceDimension.X, Is.EqualTo(spriteFileContent.SourceDimension.X));
            Assert.That(actual.SourceDimension.Y, Is.EqualTo(spriteFileContent.SourceDimension.Y));
            Assert.That(actual.SourceAnchor.X, Is.EqualTo(spriteFileContent.SourceAnchor.X));
            Assert.That(actual.SourceAnchor.Y, Is.EqualTo(spriteFileContent.SourceAnchor.Y));
            Assert.That(actual.PixelsPerUnit, Is.EqualTo(spriteFileContent.PixelsPerUnit));
        }
    }
}