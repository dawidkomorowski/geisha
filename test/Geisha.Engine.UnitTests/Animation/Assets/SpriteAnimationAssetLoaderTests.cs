using System;
using System.IO;
using Geisha.Common.FileSystem;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Animation.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Animation.Assets
{
    [TestFixture]
    public class SpriteAnimationAssetLoaderTests
    {
        [Test]
        public void LoadAsset_ShouldLoadSpriteAnimationFromFile()
        {
            // Arrange
            const string assetFilePath = @"some_directory\sprite_animation_file_path";
            const int duration = 123;
            var sprite1AssetId = AssetId.CreateUnique();
            var sprite2AssetId = AssetId.CreateUnique();
            var sprite3AssetId = AssetId.CreateUnique();

            var spriteAnimationAssetContent = new SpriteAnimationAssetContent
            {
                AssetId = Guid.NewGuid(),
                DurationTicks = TimeSpan.FromSeconds(duration).Ticks,
                Frames = new[]
                {
                    new SpriteAnimationAssetContent.Frame
                    {
                        Duration = 1,
                        SpriteAssetId = sprite1AssetId.Value
                    },
                    new SpriteAnimationAssetContent.Frame
                    {
                        Duration = 1.5,
                        SpriteAssetId = sprite2AssetId.Value
                    },
                    new SpriteAnimationAssetContent.Frame
                    {
                        Duration = 0.5,
                        SpriteAssetId = sprite3AssetId.Value
                    }
                }
            };

            var assetInfo = new AssetInfo(AssetId.CreateUnique(), AnimationAssetTypes.SpriteAnimation, assetFilePath);
            var assetData = AssetData.CreateWithJsonContent(assetInfo.AssetId, assetInfo.AssetType, spriteAnimationAssetContent);
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;

            var sprite1 = new Sprite(Substitute.For<ITexture>());
            var sprite2 = new Sprite(Substitute.For<ITexture>());
            var sprite3 = new Sprite(Substitute.For<ITexture>());

            var assetFile = Substitute.For<IFile>();
            assetFile.OpenRead().Returns(memoryStream);
            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFile(assetFilePath).Returns(assetFile);
            var assetStore = Substitute.For<IAssetStore>();
            assetStore.GetAsset<Sprite>(sprite1AssetId).Returns(sprite1);
            assetStore.GetAsset<Sprite>(sprite2AssetId).Returns(sprite2);
            assetStore.GetAsset<Sprite>(sprite3AssetId).Returns(sprite3);

            var spriteAnimationManagedAsset = new SpriteAnimationAssetLoader(fileSystem);

            // Act
            var actual = spriteAnimationManagedAsset.LoadAsset(assetInfo, assetStore);

            // Assert
            Assert.That(actual, Is.TypeOf<SpriteAnimation>());
            var spriteAnimation = (SpriteAnimation)actual;
            Assert.That(spriteAnimation.Duration, Is.EqualTo(TimeSpan.FromSeconds(duration)));
            Assert.That(spriteAnimation.Frames, Has.Count.EqualTo(3));
            Assert.That(spriteAnimation.Frames[0].Duration, Is.EqualTo(1));
            Assert.That(spriteAnimation.Frames[0].Sprite, Is.EqualTo(sprite1));
            Assert.That(spriteAnimation.Frames[1].Duration, Is.EqualTo(1.5));
            Assert.That(spriteAnimation.Frames[1].Sprite, Is.EqualTo(sprite2));
            Assert.That(spriteAnimation.Frames[2].Duration, Is.EqualTo(0.5));
            Assert.That(spriteAnimation.Frames[2].Sprite, Is.EqualTo(sprite3));
        }
    }
}