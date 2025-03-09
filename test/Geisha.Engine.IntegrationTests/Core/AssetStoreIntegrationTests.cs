using System;
using System.Linq;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Assets;
using Geisha.IntegrationTestsData;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Core
{
    public sealed class AssetStoreIntegrationTestsSut
    {
        public AssetStoreIntegrationTestsSut(IAssetStore assetStore)
        {
            AssetStore = assetStore;
        }

        public IAssetStore AssetStore { get; }
    }

    [TestFixture]
    public class AssetStoreIntegrationTests : IntegrationTests<AssetStoreIntegrationTestsSut>
    {
        [Test]
        public void RegisterAssets_ShouldRegisterAssets_GivenPathToDirectoryWithAssets()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory("Assets");

            // Assume
            Assert.That(SystemUnderTest.AssetStore.GetRegisteredAssets(), Is.Empty);

            // Act
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Assert
            var registeredAssets = SystemUnderTest.AssetStore.GetRegisteredAssets().ToList();

            var inputMappingAssetInfo = registeredAssets.Single(i => i.AssetId == AssetsIds.TestInputMapping);
            Assert.That(inputMappingAssetInfo.AssetType, Is.EqualTo(InputAssetTypes.InputMapping));

            var soundAssetInfo = registeredAssets.Single(i => i.AssetId == AssetsIds.TestSound);
            Assert.That(soundAssetInfo.AssetType, Is.EqualTo(AudioAssetTypes.Sound));

            var textureAssetInfo = registeredAssets.Single(i => i.AssetId == AssetsIds.TestTexture);
            Assert.That(textureAssetInfo.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));

            var spriteAssetInfo = registeredAssets.Single(i => i.AssetId == AssetsIds.TestSprite);
            Assert.That(spriteAssetInfo.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));

            var spriteAnimationAssetInfo = registeredAssets.Single(i => i.AssetId == AssetsIds.TestSpriteAnimation);
            Assert.That(spriteAnimationAssetInfo.AssetType, Is.EqualTo(AnimationAssetTypes.SpriteAnimation));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_InputMapping()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory("Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var inputMapping = SystemUnderTest.AssetStore.GetAsset<InputMapping>(AssetsIds.TestInputMapping);

            // Assert
            Assert.That(inputMapping, Is.Not.Null);

            Assert.That(inputMapping.ActionMappings.Count, Is.EqualTo(2));
            // Action mapping
            Assert.That(inputMapping.ActionMappings[0].ActionName, Is.EqualTo("Jump"));
            Assert.That(inputMapping.ActionMappings[0].HardwareActions.Count, Is.EqualTo(1));
            Assert.That(inputMapping.ActionMappings[0].HardwareActions[0].HardwareInputVariant,
                Is.EqualTo(HardwareInputVariant.CreateKeyboardVariant(Key.Space)));

            Assert.That(inputMapping.ActionMappings[1].ActionName, Is.EqualTo("Fire"));
            Assert.That(inputMapping.ActionMappings[1].HardwareActions.Count, Is.EqualTo(1));
            Assert.That(inputMapping.ActionMappings[1].HardwareActions[0].HardwareInputVariant,
                Is.EqualTo(HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.LeftButton)));

            Assert.That(inputMapping.AxisMappings.Count, Is.EqualTo(4));
            // Axis mapping 1
            Assert.That(inputMapping.AxisMappings[0].AxisName, Is.EqualTo("MoveForward"));
            Assert.That(inputMapping.AxisMappings[0].HardwareAxes.Count, Is.EqualTo(2));
            Assert.That(inputMapping.AxisMappings[0].HardwareAxes[0].HardwareInputVariant, Is.EqualTo(HardwareInputVariant.CreateKeyboardVariant(Key.W)));
            Assert.That(inputMapping.AxisMappings[0].HardwareAxes[0].Scale, Is.EqualTo(1));
            Assert.That(inputMapping.AxisMappings[0].HardwareAxes[1].HardwareInputVariant, Is.EqualTo(HardwareInputVariant.CreateKeyboardVariant(Key.S)));
            Assert.That(inputMapping.AxisMappings[0].HardwareAxes[1].Scale, Is.EqualTo(-1));
            // Axis mapping 2
            Assert.That(inputMapping.AxisMappings[1].AxisName, Is.EqualTo("MoveRight"));
            Assert.That(inputMapping.AxisMappings[1].HardwareAxes.Count, Is.EqualTo(2));
            Assert.That(inputMapping.AxisMappings[1].HardwareAxes[0].HardwareInputVariant, Is.EqualTo(HardwareInputVariant.CreateKeyboardVariant(Key.D)));
            Assert.That(inputMapping.AxisMappings[1].HardwareAxes[0].Scale, Is.EqualTo(1));
            Assert.That(inputMapping.AxisMappings[1].HardwareAxes[1].HardwareInputVariant, Is.EqualTo(HardwareInputVariant.CreateKeyboardVariant(Key.A)));
            Assert.That(inputMapping.AxisMappings[1].HardwareAxes[1].Scale, Is.EqualTo(-1));
            // Axis mapping 3
            Assert.That(inputMapping.AxisMappings[2].AxisName, Is.EqualTo("LookUp"));
            Assert.That(inputMapping.AxisMappings[2].HardwareAxes.Count, Is.EqualTo(1));
            Assert.That(inputMapping.AxisMappings[2].HardwareAxes[0].HardwareInputVariant,
                Is.EqualTo(HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.AxisX)));
            Assert.That(inputMapping.AxisMappings[2].HardwareAxes[0].Scale, Is.EqualTo(1));
            // Axis mapping 4
            Assert.That(inputMapping.AxisMappings[3].AxisName, Is.EqualTo("LookRight"));
            Assert.That(inputMapping.AxisMappings[3].HardwareAxes.Count, Is.EqualTo(1));
            Assert.That(inputMapping.AxisMappings[3].HardwareAxes[0].HardwareInputVariant,
                Is.EqualTo(HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.AxisY)));
            Assert.That(inputMapping.AxisMappings[3].HardwareAxes[0].Scale, Is.EqualTo(1));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Sound()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory("Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var sound = SystemUnderTest.AssetStore.GetAsset<ISound>(AssetsIds.TestSound);

            // Assert
            Assert.That(sound, Is.Not.Null);
            Assert.That(sound.Format, Is.EqualTo(SoundFormat.Mp3));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Texture()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory("Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var texture = SystemUnderTest.AssetStore.GetAsset<ITexture>(AssetsIds.TestTexture);

            // Assert
            Assert.That(texture, Is.Not.Null);
            Assert.That(texture.Dimensions, Is.EqualTo(new Vector2(10, 10)));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Sprite()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory("Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSprite);

            // Assert
            Assert.That(sprite, Is.Not.Null);
            Assert.That(sprite.PixelsPerUnit, Is.EqualTo(1));
            Assert.That(sprite.Pivot, Is.EqualTo(new Vector2(5, 5)));
            Assert.That(sprite.SourceDimensions, Is.EqualTo(new Vector2(10, 10)));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(sprite.SourceTexture), Is.EqualTo(AssetsIds.TestTexture));
            Assert.That(sprite.SourceUV, Is.EqualTo(new Vector2(0, 0)));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_SpriteAnimation()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory("Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var spriteAnimation = SystemUnderTest.AssetStore.GetAsset<SpriteAnimation>(AssetsIds.TestSpriteAnimation);

            // Assert
            Assert.That(spriteAnimation, Is.Not.Null);
            Assert.That(spriteAnimation.Duration, Is.EqualTo(TimeSpan.FromSeconds(2)));
            Assert.That(spriteAnimation.Frames, Has.Count.EqualTo(3));
            Assert.That(spriteAnimation.Frames[0].Duration, Is.EqualTo(1));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(spriteAnimation.Frames[0].Sprite), Is.EqualTo(AssetsIds.TestSpriteAnimationFrame1));
            Assert.That(spriteAnimation.Frames[1].Duration, Is.EqualTo(1.5));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(spriteAnimation.Frames[1].Sprite), Is.EqualTo(AssetsIds.TestSpriteAnimationFrame2));
            Assert.That(spriteAnimation.Frames[2].Duration, Is.EqualTo(0.5));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(spriteAnimation.Frames[2].Sprite), Is.EqualTo(AssetsIds.TestSpriteAnimationFrame3));
        }
    }
}