using System;
using System.IO;
using System.Threading;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
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
        protected override bool ShowDebugWindow => true;

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

            SystemUnderTest.AssetStore.RegisterAssets(Utils.GetPathUnderTestDirectory(@"Assets"));
        }

        public sealed class RenderingTestCase
        {
            public string Name { get; set; } = string.Empty;
            public string ExpectedReferenceImageFile { get; set; } = string.Empty;
            public Action<Scene, EntityFactory> SetUpScene { get; set; } = (scene, entityFactory) => { };

            public override string ToString() => Name;
        }

        public static RenderingTestCase[] RenderingTestCases =
        {
            new RenderingTestCase
            {
                Name = "Rectangle rendering",
                ExpectedReferenceImageFile = "Rectangles.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    entityFactory.CreateCamera(scene);
                    entityFactory.CreateRectangle(scene, new Vector2(40, 20), Color.FromArgb(255, 255, 0, 0), translation: new Vector2(50, -50));
                    entityFactory.CreateRectangle(scene, new Vector2(20, 40), Color.FromArgb(255, 0, 0, 255), translation: new Vector2(-50, 50));
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 0, 255, 0), fillInterior: true);
                }
            }
        };

        [TestCaseSource(nameof(RenderingTestCases))]
        public void RenderScene_ShouldRenderImageSameAsReferenceImage_GivenScene(RenderingTestCase testCase)
        {
            // Arrange
            var scene = new Scene();
            var entityFactory = new EntityFactory(SystemUnderTest.AssetStore);

            testCase.SetUpScene(scene, entityFactory);

            // Act
            SystemUnderTest.RenderingSystem.RenderScene(scene);

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Assert
            using var memoryStream = new MemoryStream();
            SystemUnderTest.RenderingBackend.Renderer2D.CaptureScreenShotAsPng(memoryStream);

            const string tmpOutputPath = @"C:\Users\Dawid Komorowski\Downloads\RenderingTests";
            var file = Path.Combine(tmpOutputPath, testCase.ExpectedReferenceImageFile);
            using (var fileStream = File.Create(file))
            {
                SystemUnderTest.RenderingBackend.Renderer2D.CaptureScreenShotAsPng(fileStream);
            }

            var referenceImageFilePath = Utils.GetPathUnderTestDirectory(Path.Combine("Rendering", "ReferenceImages", testCase.ExpectedReferenceImageFile));

            Assert.That(memoryStream.GetBuffer(), Is.EqualTo(File.ReadAllBytes(referenceImageFilePath)));
            Assert.That(memoryStream.ToArray(), Is.EqualTo(File.ReadAllBytes(referenceImageFilePath)));
        }

        public sealed class EntityFactory
        {
            private readonly IAssetStore _assetStore;

            public EntityFactory(IAssetStore assetStore)
            {
                _assetStore = assetStore;
            }

            public Entity CreateCamera(Scene scene)
            {
                var entity = new Entity();
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = Vector2.Zero,
                    Rotation = 0,
                    Scale = Vector2.One
                });
                entity.AddComponent(new CameraComponent
                {
                    ViewRectangle = new Vector2(200, 200)
                });
                scene.AddEntity(entity);

                return entity;
            }

            public Entity CreateRectangle(Scene scene, Vector2 dimension, Color color, bool fillInterior = false, Vector2? translation = null,
                double rotation = 0, Vector2? scale = null)
            {
                var entity = new Entity();
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = translation ?? Vector2.Zero,
                    Rotation = rotation,
                    Scale = scale ?? Vector2.One
                });
                entity.AddComponent(new RectangleRendererComponent
                {
                    Dimension = dimension,
                    Color = color,
                    FillInterior = fillInterior
                });
                scene.AddEntity(entity);

                return entity;
            }
        }
    }
}