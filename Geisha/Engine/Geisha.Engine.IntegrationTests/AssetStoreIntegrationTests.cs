using System.Linq;
using Autofac;
using Geisha.Common.TestUtils;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Audio;
using Geisha.Framework.Rendering;
using Geisha.Framework.Rendering.DirectX.IntegrationTests;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
{
    [TestFixture]
    public class AssetStoreIntegrationTests : IntegrationTests<IAssetStore>
    {
        protected override void RegisterComponents(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<IntegrationTestsWindow>().As<IWindow>().SingleInstance();
        }

        [Test]
        public void RegisterAssets_ShouldRegisterAssetTypesForWhichDiscoveryRulesAreProvided_GivenPathToDirectoryWithAssets()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"TestData\Assets");

            // Assume
            Assume.That(SystemUnderTest.GetRegisteredAssets(), Is.Empty);

            // Act
            SystemUnderTest.RegisterAssets(assetsDirectoryPath);

            // Assert
            var registeredAssets = SystemUnderTest.GetRegisteredAssets().ToList();
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
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"TestData\Assets");
            SystemUnderTest.RegisterAssets(assetsDirectoryPath);

            // Act
            var inputMapping = SystemUnderTest.GetAsset<InputMapping>(AssetsIds.TestInputMapping);

            // Assert
            Assert.That(inputMapping, Is.Not.Null);
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Sound()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"TestData\Assets");
            SystemUnderTest.RegisterAssets(assetsDirectoryPath);

            // Act
            var sound = SystemUnderTest.GetAsset<ISound>(AssetsIds.TestSound);

            // Assert
            Assert.That(sound, Is.Not.Null);
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Texture()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"TestData\Assets");
            SystemUnderTest.RegisterAssets(assetsDirectoryPath);

            // Act
            var texture = SystemUnderTest.GetAsset<ITexture>(AssetsIds.TestTexture);

            // Assert
            Assert.That(texture, Is.Not.Null);
        }

        [Test]
        public void GetAsset_ShouldLoadAndReturn_Sprite()
        {
            // Arrange
            var assetsDirectoryPath = Utils.GetPathUnderTestDirectory(@"TestData\Assets");
            SystemUnderTest.RegisterAssets(assetsDirectoryPath);

            // Act
            var sprite = SystemUnderTest.GetAsset<Sprite>(AssetsIds.TestSprite);

            // Assert
            Assert.That(sprite, Is.Not.Null);
        }
    }
}