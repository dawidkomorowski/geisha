using System;
using System.Threading;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using Geisha.IntegrationTestsData;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests.Rendering
{
    internal sealed class RenderingSystemIntegrationTestsSut
    {
        public RenderingSystemIntegrationTestsSut(IAssetStore assetStore, IRenderingBackend renderingBackend, IRenderingSystem renderingSystem)
        {
            AssetStore = assetStore;
            RenderingBackend = renderingBackend;
            RenderingSystem = renderingSystem;
        }

        public IAssetStore AssetStore { get; }
        public IRenderingBackend RenderingBackend { get; }
        public IRenderingSystem RenderingSystem { get; }
    }

    [TestFixture]
    internal class RenderingSystemIntegrationTests : IntegrationTests<RenderingSystemIntegrationTestsSut>
    {
        private TemporaryDirectory _temporaryDirectory = null!;

        protected override bool ShowWindow => true;

        protected override void ConfigureRendering(RenderingConfiguration.IBuilder builder)
        {
            base.ConfigureRendering(builder);

            builder
                .WithScreenWidth(200)
                .WithScreenHeight(200);
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
            var scene = new Scene();

            var cameraEntity = new Entity();
            cameraEntity.AddComponent(new Transform2DComponent
            {
                Translation = Vector2.Zero,
                Rotation = 0,
                Scale = Vector2.One
            });
            cameraEntity.AddComponent(new CameraComponent
            {
                ViewRectangle = new Vector2(200, 200)
            });
            scene.AddEntity(cameraEntity);

            var rectangleEntity = new Entity();
            rectangleEntity.AddComponent(new Transform2DComponent
            {
                Translation = Vector2.Zero,
                Rotation = 0,
                Scale = Vector2.One
            });
            rectangleEntity.AddComponent(new RectangleRendererComponent
            {
                Dimension = new Vector2(40, 20),
                Color = Color.FromArgb(255, 255, 0, 0)
            });
            scene.AddEntity(rectangleEntity);

            var rectangleEntity2 = new Entity();
            rectangleEntity2.AddComponent(new Transform2DComponent
            {
                Translation = Vector2.Zero,
                Rotation = 0,
                Scale = Vector2.One
            });
            rectangleEntity2.AddComponent(new RectangleRendererComponent
            {
                Dimension = new Vector2(200, 200),
                Color = Color.FromArgb(255, 0, 0, 255)
            });
            scene.AddEntity(rectangleEntity2);

            // Act
            SystemUnderTest.RenderingSystem.RenderScene(scene);

            // Assert
            var sprite = SystemUnderTest.AssetStore.GetAsset<Sprite>(AssetsIds.TestSpriteSheetSprite);
            SystemUnderTest.RenderingBackend.Renderer2D.CaptureScreenShotPng(sprite);

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}