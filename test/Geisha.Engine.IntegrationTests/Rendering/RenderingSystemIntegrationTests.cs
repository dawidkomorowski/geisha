using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Rendering
{
    internal sealed class RenderingSystemIntegrationTestsSut
    {
        public RenderingSystemIntegrationTestsSut(IAssetStore assetStore, IRenderingSystem renderingSystem)
        {
            AssetStore = assetStore;
            RenderingSystem = renderingSystem;
        }

        public IAssetStore AssetStore { get; }
        public IRenderingSystem RenderingSystem { get; }
    }

    [TestFixture]
    internal class RenderingSystemIntegrationTests : IntegrationTests<RenderingSystemIntegrationTestsSut>
    {
        private TemporaryDirectory _temporaryDirectory = null!;

        protected override void ConfigureRendering(RenderingConfiguration.IBuilder builder)
        {
            base.ConfigureRendering(builder);

            builder
                .WithScreenWidth(100)
                .WithScreenHeight(100);
        }

        public override void SetUp()
        {
            base.SetUp();
            _temporaryDirectory = new TemporaryDirectory();

            SystemUnderTest.AssetStore.RegisterAssets(Utils.GetPathUnderTestDirectory(@"Assets"));
        }

        public override void TearDown()
        {
            base.TearDown();
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void TODO()
        {
            // Arrange
            // Act
            // Assert
        }
    }
}