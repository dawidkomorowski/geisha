using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.TestUtils;
using Geisha.Engine.Audio;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering;
using Geisha.IntegrationTestsData;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
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
        public void RegisterAssets_ShouldRegisterAssetTypesForWhichDiscoveryRulesAreProvided_GivenPathToDirectoryWithAssets()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"Assets");

            // Assume
            Assume.That(SystemUnderTest.AssetStore.GetRegisteredAssets(), Is.Empty);

            // Act
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Assert
            var registeredAssets = SystemUnderTest.AssetStore.GetRegisteredAssets().ToList();
            Assert.That(registeredAssets, Has.Exactly(4).Items);

            var inputMappingAssetInfo = registeredAssets.Single(i => i.AssetType == typeof(InputMapping));
            Assert.That(inputMappingAssetInfo.AssetId, Is.EqualTo(AssetsIds.TestInputMapping));

            var soundAssetInfo = registeredAssets.Single(i => i.AssetType == typeof(ISound));
            Assert.That(soundAssetInfo.AssetId, Is.EqualTo(AssetsIds.TestSound));

            var textureAssetInfo = registeredAssets.Single(i => i.AssetType == typeof(ITexture));
            Assert.That(textureAssetInfo.AssetId, Is.EqualTo(AssetsIds.TestTexture));

            var spriteAssetInfo = registeredAssets.Single(i => i.AssetType == typeof(Sprite));
            Assert.That(spriteAssetInfo.AssetId, Is.EqualTo(AssetsIds.TestSprite));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_InputMapping()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"Assets");
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
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"Assets");
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
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var texture = SystemUnderTest.AssetStore.GetAsset<ITexture>(AssetsIds.TestTexture);

            // Assert
            Assert.That(texture, Is.Not.Null);
            Assert.That(texture.Dimension, Is.EqualTo(new Vector2(10, 10)));
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Sprite()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"Assets");
            SystemUnderTest.AssetStore.RegisterAssets(assetsDirectoryPath);

            // Act
            var sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSprite);

            // Assert
            Assert.That(sprite, Is.Not.Null);
            Assert.That(sprite.PixelsPerUnit, Is.EqualTo(1));
            Assert.That(sprite.SourceAnchor, Is.EqualTo(new Vector2(5, 5)));
            Assert.That(sprite.SourceDimension, Is.EqualTo(new Vector2(10, 10)));
            Assert.That(SystemUnderTest.AssetStore.GetAssetId(sprite.SourceTexture), Is.EqualTo(AssetsIds.TestTexture));
            Assert.That(sprite.SourceUV, Is.EqualTo(new Vector2(0, 0)));
        }
    }
}