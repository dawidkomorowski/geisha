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
        private const string Background = "Background";
        private const string Foreground = "Foreground";

        protected override bool ShowDebugWindow => true;

        protected override void ConfigureRendering(RenderingConfiguration.IBuilder builder)
        {
            base.ConfigureRendering(builder);

            builder
                .WithScreenWidth(200)
                .WithScreenHeight(200)
                .WithSortingLayersOrder(new[] { Background, RenderingConfiguration.DefaultSortingLayerName, Foreground });
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

                    // Basic scenario
                    entityFactory.CreateRectangle(scene, new Vector2(40, 20), Color.FromArgb(255, 255, 0, 0), translation: new Vector2(50, -50));
                    entityFactory.CreateRectangle(scene, new Vector2(20, 40), Color.FromArgb(255, 0, 0, 255), translation: new Vector2(-50, 50));
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 0, 255, 0), fillInterior: true);

                    // 2D transformations
                    entityFactory.CreateRectangle(scene, new Vector2(100, 100), Color.FromArgb(255, 255, 255, 0), fillInterior: true,
                        translation: new Vector2(50, 50), rotation: Angle.Deg2Rad(45), scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateRectangle(scene, new Vector2(200, 100), Color.FromArgb(255, 255, 0, 255), fillInterior: true,
                        translation: new Vector2(-50, -50), rotation: Angle.Deg2Rad(20), scale: new Vector2(0.2, 0.3));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateRectangle(scene, new Vector2(200, 200), Color.FromArgb(255, 0, 0, 0), true);
                    visibilityEntity.GetComponent<RectangleRendererComponent>().Visible = false;
                }
            },
            new RenderingTestCase
            {
                Name = "Ellipse rendering",
                ExpectedReferenceImageFile = "Ellipses.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Basic scenario
                    entityFactory.CreateEllipse(scene, 20, 10, Color.FromArgb(255, 255, 0, 0), translation: new Vector2(50, -50));
                    entityFactory.CreateEllipse(scene, 10, 20, Color.FromArgb(255, 0, 0, 255), translation: new Vector2(-50, 50));
                    entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 0, 255, 0), fillInterior: true);

                    // 2D transformations
                    entityFactory.CreateEllipse(scene, 100, 100, Color.FromArgb(255, 255, 255, 0), fillInterior: true,
                        translation: new Vector2(50, 50), rotation: Angle.Deg2Rad(45), scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateEllipse(scene, 200, 100, Color.FromArgb(255, 255, 0, 255), fillInterior: true,
                        translation: new Vector2(-50, -50), rotation: Angle.Deg2Rad(20), scale: new Vector2(0.2, 0.3));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateEllipse(scene, 200, 200, Color.FromArgb(255, 0, 0, 0), true);
                    visibilityEntity.GetComponent<EllipseRendererComponent>().Visible = false;
                }
            },
            new RenderingTestCase
            {
                Name = "Sprite rendering",
                ExpectedReferenceImageFile = "Sprites.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Basic scenario
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.Part0Sprite, translation: new Vector2(-39, 39));
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.Part1Sprite, translation: new Vector2(39, 39));
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.Part2Sprite, translation: new Vector2(-39, -39));
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.Part3Sprite, translation: new Vector2(39, -39));
                    entityFactory.CreateSprite(scene, AssetsIds.Sprites.Sample01);

                    // 2D transformations
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(-75, 75), rotation: Angle.Deg2Rad(45),
                        scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(75, 75), rotation: Angle.Deg2Rad(-45),
                        scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(75, -75), rotation: Angle.Deg2Rad(-135),
                        scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(-75, -75), rotation: Angle.Deg2Rad(135),
                        scale: new Vector2(0.2, 0.2));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, scale: new Vector2(2, 2));
                    visibilityEntity.GetComponent<SpriteRendererComponent>().Visible = false;
                }
            },
            new RenderingTestCase
            {
                Name = "Text rendering",
                ExpectedReferenceImageFile = "Texts.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Basic scenario
                    entityFactory.CreateText(scene, "Geisha Engine", FontSize.FromDips(20), Color.FromArgb(255, 0, 0, 0), translation: new Vector2(-70, 70));
                    entityFactory.CreateText(scene, "Red", FontSize.FromDips(30), Color.FromArgb(255, 255, 0, 0), translation: new Vector2(-70, 30));
                    entityFactory.CreateText(scene, "Green", FontSize.FromDips(30), Color.FromArgb(255, 0, 255, 0), translation: new Vector2(-70, 0));
                    entityFactory.CreateText(scene, "Blue", FontSize.FromDips(30), Color.FromArgb(255, 0, 0, 255), translation: new Vector2(-70, -30));

                    // 2D transformations
                    entityFactory.CreateText(scene, "Transformed", FontSize.FromDips(30), Color.FromArgb(255, 0, 255, 255), translation: new Vector2(90, 30),
                        rotation: Angle.Deg2Rad(-90), scale: new Vector2(0.5, 2.0));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateText(scene, "Invisible", FontSize.FromDips(30), Color.FromArgb(255, 0, 0, 0),
                        translation: new Vector2(-100, 100));
                    visibilityEntity.GetComponent<TextRendererComponent>().Visible = false;
                }
            },
            new RenderingTestCase
            {
                Name = "Sorting layers",
                ExpectedReferenceImageFile = "SortingLayers.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 0, 255, 0), fillInterior: true,
                        translation: new Vector2(-65, 65));
                    var r1 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 255, 0, 0), fillInterior: true,
                        translation: new Vector2(-75, 75));
                    r1.GetComponent<RectangleRendererComponent>().SortingLayerName = Background;
                    var r3 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 0, 0, 255), fillInterior: true,
                        translation: new Vector2(-55, 55));
                    r3.GetComponent<RectangleRendererComponent>().SortingLayerName = Foreground;

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 0, 255, 0), fillInterior: true,
                        translation: new Vector2(65, 65));
                    var e1 = entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 255, 0, 0), fillInterior: true,
                        translation: new Vector2(75, 75));
                    e1.GetComponent<EllipseRendererComponent>().SortingLayerName = Background;
                    var e3 = entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 0, 0, 255), fillInterior: true,
                        translation: new Vector2(55, 55));
                    e3.GetComponent<EllipseRendererComponent>().SortingLayerName = Foreground;

                    // Sprite
                    entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarEyeF4, scale: new Vector2(2, 2));
                    entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarMouthF1, scale: new Vector2(2, 2));
                    var s1 = entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarHeadF3, scale: new Vector2(2, 2));
                    s1.GetComponent<SpriteRendererComponent>().SortingLayerName = Background;
                    var s3 = entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarHairF13, scale: new Vector2(2, 2));
                    s3.GetComponent<SpriteRendererComponent>().SortingLayerName = Foreground;

                    // Text
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 0, 255, 0), translation: new Vector2(-31, -66));
                    var t1 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 255, 0, 0),
                        translation: new Vector2(-30, -65));
                    t1.GetComponent<TextRendererComponent>().SortingLayerName = Background;
                    var t3 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 0, 0, 255),
                        translation: new Vector2(-32, -67));
                    t3.GetComponent<TextRendererComponent>().SortingLayerName = Foreground;
                }
            },
            new RenderingTestCase
            {
                Name = "Order in layer",
                ExpectedReferenceImageFile = "SortingLayers.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 0, 255, 0), fillInterior: true,
                        translation: new Vector2(-65, 65));
                    var r1 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 255, 0, 0), fillInterior: true,
                        translation: new Vector2(-75, 75));
                    r1.GetComponent<RectangleRendererComponent>().OrderInLayer = -1;
                    var r3 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.FromArgb(255, 0, 0, 255), fillInterior: true,
                        translation: new Vector2(-55, 55));
                    r3.GetComponent<RectangleRendererComponent>().OrderInLayer = 1;

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 0, 255, 0), fillInterior: true,
                        translation: new Vector2(65, 65));
                    var e1 = entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 255, 0, 0), fillInterior: true,
                        translation: new Vector2(75, 75));
                    e1.GetComponent<EllipseRendererComponent>().OrderInLayer = -1;
                    var e3 = entityFactory.CreateEllipse(scene, 15, 15, Color.FromArgb(255, 0, 0, 255), fillInterior: true,
                        translation: new Vector2(55, 55));
                    e3.GetComponent<EllipseRendererComponent>().OrderInLayer = 1;

                    // Sprite
                    entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarEyeF4, scale: new Vector2(2, 2));
                    entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarMouthF1, scale: new Vector2(2, 2));
                    var s1 = entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarHeadF3, scale: new Vector2(2, 2));
                    s1.GetComponent<SpriteRendererComponent>().OrderInLayer = -1;
                    var s3 = entityFactory.CreateSprite(scene, AssetsIds.Sprites.AvatarHairF13, scale: new Vector2(2, 2));
                    s3.GetComponent<SpriteRendererComponent>().OrderInLayer = 1;

                    // Text
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 0, 255, 0), translation: new Vector2(-31, -66));
                    var t1 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 255, 0, 0),
                        translation: new Vector2(-30, -65));
                    t1.GetComponent<TextRendererComponent>().OrderInLayer = -1;
                    var t3 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 0, 0, 255),
                        translation: new Vector2(-32, -67));
                    t3.GetComponent<TextRendererComponent>().OrderInLayer = 1;
                }
            },
            new RenderingTestCase
            {
                Name = "Camera transformation",
                ExpectedReferenceImageFile = "CameraTransformation.png",
                SetUpScene = (scene, entityFactory) =>
                {
                    var camera = entityFactory.CreateCamera(scene);
                    var cameraTransform = camera.GetComponent<Transform2DComponent>();
                    cameraTransform.Translation = new Vector2(100, 100);
                    cameraTransform.Rotation = Angle.Deg2Rad(-45);
                    cameraTransform.Scale = new Vector2(3, 3);

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(60, 30), Color.FromArgb(255, 255, 0, 0), fillInterior: true,
                        translation: new Vector2(-75, 75));

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 30, 15, Color.FromArgb(255, 0, 255, 0), fillInterior: true,
                        translation: new Vector2(75, 75));

                    // Sprite
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite);

                    // Text
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(255, 0, 0, 255), translation: new Vector2(-31, -66));
                }
            }
        };

        [TestCaseSource(nameof(RenderingTestCases))]
        public void RenderScene_ShouldRenderImageSameAsReferenceImage(RenderingTestCase testCase)
        {
            // Arrange
            var scene = new Scene();
            var entityFactory = new EntityFactory(SystemUnderTest.AssetStore);

            testCase.SetUpScene(scene, entityFactory);

            // Act
            SystemUnderTest.RenderingSystem.RenderScene(scene);

            Thread.Sleep(TimeSpan.FromSeconds(1));

            // Assert
            using var memoryStream = new MemoryStream();
            SystemUnderTest.RenderingBackend.Renderer2D.CaptureScreenShotAsPng(memoryStream);

            // TODO Remove code for saving reference images.
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

            public Entity CreateEllipse(Scene scene, double radiusX, double radiusY, Color color, bool fillInterior = false, Vector2? translation = null,
                double rotation = 0, Vector2? scale = null)
            {
                var entity = new Entity();
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = translation ?? Vector2.Zero,
                    Rotation = rotation,
                    Scale = scale ?? Vector2.One
                });
                entity.AddComponent(new EllipseRendererComponent
                {
                    RadiusX = radiusX,
                    RadiusY = radiusY,
                    Color = color,
                    FillInterior = fillInterior
                });
                scene.AddEntity(entity);

                return entity;
            }

            public Entity CreateSprite(Scene scene, AssetId spriteAssetId, Vector2? translation = null, double rotation = 0, Vector2? scale = null)
            {
                var entity = new Entity();
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = translation ?? Vector2.Zero,
                    Rotation = rotation,
                    Scale = scale ?? Vector2.One
                });
                entity.AddComponent(new SpriteRendererComponent
                {
                    Sprite = _assetStore.GetAsset<Sprite>(spriteAssetId)
                });
                scene.AddEntity(entity);

                return entity;
            }

            public Entity CreateText(Scene scene, string text, FontSize fontSize, Color color, Vector2? translation = null, double rotation = 0,
                Vector2? scale = null)
            {
                var entity = new Entity();
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = translation ?? Vector2.Zero,
                    Rotation = rotation,
                    Scale = scale ?? Vector2.One
                });
                entity.AddComponent(new TextRendererComponent
                {
                    Text = text,
                    FontSize = fontSize,
                    Color = color
                });
                scene.AddEntity(entity);

                return entity;
            }
        }
    }
}