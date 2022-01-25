using System.IO;
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
    public class SpriteAssetLoaderTests
    {
        [Test]
        public void LoadAsset_ShouldLoadSpriteFromFile()
        {
            // Arrange
            const string assetFilePath = @"some_directory\sprite_file_path";
            var textureAssetId = AssetId.CreateUnique();

            var spriteAssetContent = new SpriteAssetContent
            {
                TextureAssetId = textureAssetId.Value,
                SourceUV = new SerializableVector2 { X = 123.456, Y = 234.567 },
                SourceDimensions = new SerializableVector2 { X = 345.456, Y = 456.567 },
                SourceAnchor = new SerializableVector2 { X = 567.678, Y = 678.789 },
                PixelsPerUnit = 123.456
            };

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), RenderingAssetTypes.Sprite, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, spriteAssetContent);
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            var assetFile = Substitute.For<IFile>();
            assetFile.OpenRead().Returns(memoryStream);
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFile(assetFilePath).Returns(assetFile);

            var assetStore = Substitute.For<IAssetStore>();
            var texture = Substitute.For<ITexture>();
            assetStore.GetAsset<ITexture>(textureAssetId).Returns(texture);

            var spriteAssetLoader = new SpriteAssetLoader(fileSystem);

            // Act
            var actual = (Sprite)spriteAssetLoader.LoadAsset(assetInfo, assetStore);

            // Assert
            Assert.That(actual.SourceTexture, Is.EqualTo(texture));
            Assert.That(actual.SourceUV.X, Is.EqualTo(spriteAssetContent.SourceUV.X));
            Assert.That(actual.SourceUV.Y, Is.EqualTo(spriteAssetContent.SourceUV.Y));
            Assert.That(actual.SourceDimensions.X, Is.EqualTo(spriteAssetContent.SourceDimensions.X));
            Assert.That(actual.SourceDimensions.Y, Is.EqualTo(spriteAssetContent.SourceDimensions.Y));
            Assert.That(actual.SourceAnchor.X, Is.EqualTo(spriteAssetContent.SourceAnchor.X));
            Assert.That(actual.SourceAnchor.Y, Is.EqualTo(spriteAssetContent.SourceAnchor.Y));
            Assert.That(actual.PixelsPerUnit, Is.EqualTo(spriteAssetContent.PixelsPerUnit));
        }
    }
}