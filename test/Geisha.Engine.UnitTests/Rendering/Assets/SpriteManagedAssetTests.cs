using System;
using System.Diagnostics;
using System.Text.Json;
using Geisha.Common.FileSystem;
using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Assets
{
    [TestFixture]
    public class SpriteManagedAssetTests
    {
        [Test]
        public void Load_ShouldLoadSpriteFromFile()
        {
            // Arrange
            const string spriteFilePath = @"some_directory\sprite_file_path";
            var textureAssetId = AssetId.CreateUnique();

            var spriteFileContent = new SpriteFileContent
            {
                AssetId = Guid.NewGuid(),
                TextureAssetId = textureAssetId.Value,
                SourceUV = new SerializableVector2 {X = 123.456, Y = 234.567},
                SourceDimension = new SerializableVector2 {X = 345.456, Y = 456.567},
                SourceAnchor = new SerializableVector2 {X = 567.678, Y = 678.789},
                PixelsPerUnit = 123.456
            };

            var json = JsonSerializer.Serialize(spriteFileContent);

            var texture = Substitute.For<ITexture>();

            var spriteFile = Substitute.For<IFile>();
            spriteFile.ReadAllText().Returns(json);
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFile(spriteFilePath).Returns(spriteFile);
            var assetStore = Substitute.For<IAssetStore>();
            assetStore.GetAsset<ITexture>(textureAssetId).Returns(texture);

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(Sprite), spriteFilePath);
            var spriteManagedAsset = new SpriteManagedAsset(assetInfo, fileSystem, assetStore);

            // Act
            spriteManagedAsset.Load();

            Debug.Assert(spriteManagedAsset.AssetInstance != null, "spriteManagedAsset.AssetInstance != null");
            var actual = (Sprite) spriteManagedAsset.AssetInstance;

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