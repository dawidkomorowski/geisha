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
            var registeredAssets = SystemUnderTest.GetRegisteredAssets();
            Assert.That(registeredAssets, Has.Exactly(3).Items);

            var inputMapping = SystemUnderTest.GetAsset<InputMapping>(AssetsIds.TestInputMapping);
            Assert.That(inputMapping, Is.Not.Null);

            var sound = SystemUnderTest.GetAsset<ISound>(AssetsIds.TestSound);
            Assert.That(sound, Is.Not.Null);

            var texture = SystemUnderTest.GetAsset<ITexture>(AssetsIds.TestTexture);
            Assert.That(texture, Is.Not.Null);
        }
    }
}