﻿using System;
using System.IO;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using Geisha.IntegrationTestsData;
using Geisha.TestUtils;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = Geisha.Engine.Core.Math.Color;

namespace Geisha.Engine.IntegrationTests.Rendering
{
    internal sealed class RenderingSystemIntegrationTestsSut
    {
        public RenderingSystemIntegrationTestsSut(IAssetStore assetStore, IDebugRenderer debugRenderer, IRenderingBackend renderingBackend,
            IRenderingGameLoopStep renderingSystem, ISceneManagerInternal sceneManager)
        {
            AssetStore = assetStore;
            DebugRenderer = debugRenderer;
            RenderingBackend = renderingBackend;
            RenderingSystem = renderingSystem;
            SceneManager = sceneManager;
        }

        public IAssetStore AssetStore { get; }
        public IDebugRenderer DebugRenderer { get; }
        public IRenderingBackend RenderingBackend { get; }
        public IRenderingGameLoopStep RenderingSystem { get; }
        public ISceneManagerInternal SceneManager { get; }
    }

    [TestFixture]
    internal class RenderingSystemIntegrationTests : IntegrationTests<RenderingSystemIntegrationTestsSut>
    {
        private const string Background = "Background";
        private const string Foreground = "Foreground";

        private const bool SaveRenderedImages = false;
        protected override bool ShowDebugWindow => false;

        protected override RenderingConfiguration ConfigureRendering(RenderingConfiguration configuration)
        {
            return base.ConfigureRendering(configuration) with
            {
                ScreenWidth = 200,
                ScreenHeight = 200,
                SortingLayersOrder = new[] { Background, RenderingConfiguration.DefaultSortingLayerName, Foreground }
            };
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
            public Action<Scene, EntityFactory, IDebugRenderer> SetUpScene { get; set; } = (_, _, _) => { };

            public override string ToString() => Name;
        }

        public static readonly RenderingTestCase[] RenderingTestCases =
        {
            new()
            {
                Name = "Rectangle rendering",
                ExpectedReferenceImageFile = "Rectangles.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Basic scenario
                    entityFactory.CreateRectangle(scene, new Vector2(40, 20), Color.Red, translation: new Vector2(50, -50));
                    entityFactory.CreateRectangle(scene, new Vector2(20, 40), Color.Blue, translation: new Vector2(-50, 50));
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Green, fillInterior: true);

                    // 2D transformations
                    entityFactory.CreateRectangle(scene, new Vector2(100, 100), Color.FromArgb(255, 255, 255, 0), fillInterior: true,
                        translation: new Vector2(50, 50), rotation: Angle.Deg2Rad(45), scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateRectangle(scene, new Vector2(200, 100), Color.FromArgb(255, 255, 0, 255), fillInterior: true,
                        translation: new Vector2(-50, -50), rotation: Angle.Deg2Rad(20), scale: new Vector2(0.2, 0.3));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateRectangle(scene, new Vector2(200, 200), Color.Black, true);
                    visibilityEntity.GetComponent<RectangleRendererComponent>().Visible = false;
                }
            },
            new()
            {
                Name = "Ellipse rendering",
                ExpectedReferenceImageFile = "Ellipses.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Basic scenario
                    entityFactory.CreateEllipse(scene, 20, 10, Color.Red, translation: new Vector2(50, -50));
                    entityFactory.CreateEllipse(scene, 10, 20, Color.Blue, translation: new Vector2(-50, 50));
                    entityFactory.CreateEllipse(scene, 15, 15, Color.Green, fillInterior: true);

                    // 2D transformations
                    entityFactory.CreateEllipse(scene, 100, 100, Color.FromArgb(255, 255, 255, 0), fillInterior: true,
                        translation: new Vector2(50, 50), rotation: Angle.Deg2Rad(45), scale: new Vector2(0.2, 0.2));
                    entityFactory.CreateEllipse(scene, 200, 100, Color.FromArgb(255, 255, 0, 255), fillInterior: true,
                        translation: new Vector2(-50, -50), rotation: Angle.Deg2Rad(20), scale: new Vector2(0.2, 0.3));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateEllipse(scene, 200, 200, Color.Black, true);
                    visibilityEntity.GetComponent<EllipseRendererComponent>().Visible = false;
                }
            },
            new()
            {
                Name = "Sprite rendering",
                ExpectedReferenceImageFile = "Sprites.png",
                SetUpScene = (scene, entityFactory, _) =>
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
            new()
            {
                Name = "Sprite rendering - opacity",
                ExpectedReferenceImageFile = "Sprites_Opacity.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    var entity1 = entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(-50, 50));
                    entity1.GetComponent<SpriteRendererComponent>().Opacity = 0;

                    var entity2 = entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(50, 50));
                    entity2.GetComponent<SpriteRendererComponent>().Opacity = 0.25;

                    var entity3 = entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(-50, -50));
                    entity3.GetComponent<SpriteRendererComponent>().Opacity = 0.5;

                    var entity4 = entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, translation: new Vector2(50, -50));
                    entity4.GetComponent<SpriteRendererComponent>().Opacity = 1;
                }
            },
            new()
            {
                Name = "Text rendering",
                ExpectedReferenceImageFile = "Texts.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Basic scenario
                    entityFactory.CreateText(scene, "Geisha Engine", FontSize.FromDips(20), Color.Black, translation: new Vector2(-70, 70));
                    entityFactory.CreateText(scene, "Red", FontSize.FromDips(30), Color.Red, translation: new Vector2(-70, 30));
                    entityFactory.CreateText(scene, "Green", FontSize.FromDips(30), Color.Green, translation: new Vector2(-70, 0));
                    entityFactory.CreateText(scene, "Blue", FontSize.FromDips(30), Color.Blue, translation: new Vector2(-70, -30));

                    // 2D transformations
                    entityFactory.CreateText(scene, "Transformed", FontSize.FromDips(30), Color.FromArgb(255, 0, 255, 255), translation: new Vector2(90, 30),
                        rotation: Angle.Deg2Rad(-90), scale: new Vector2(0.5, 2.0));

                    // Visibility
                    var visibilityEntity = entityFactory.CreateText(scene, "Invisible", FontSize.FromDips(30), Color.Black,
                        translation: new Vector2(-100, 100));
                    visibilityEntity.GetComponent<TextRendererComponent>().Visible = false;
                }
            },
            new()
            {
                Name = "Sorting layers",
                ExpectedReferenceImageFile = "SortingLayers.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Green, fillInterior: true,
                        translation: new Vector2(-65, 65));
                    var r1 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Red, fillInterior: true,
                        translation: new Vector2(-75, 75));
                    r1.GetComponent<RectangleRendererComponent>().SortingLayerName = Background;
                    var r3 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Blue, fillInterior: true,
                        translation: new Vector2(-55, 55));
                    r3.GetComponent<RectangleRendererComponent>().SortingLayerName = Foreground;

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 15, 15, Color.Green, fillInterior: true,
                        translation: new Vector2(65, 65));
                    var e1 = entityFactory.CreateEllipse(scene, 15, 15, Color.Red, fillInterior: true,
                        translation: new Vector2(75, 75));
                    e1.GetComponent<EllipseRendererComponent>().SortingLayerName = Background;
                    var e3 = entityFactory.CreateEllipse(scene, 15, 15, Color.Blue, fillInterior: true,
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
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Green, translation: new Vector2(-31, -66));
                    var t1 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Red,
                        translation: new Vector2(-30, -65));
                    t1.GetComponent<TextRendererComponent>().SortingLayerName = Background;
                    var t3 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Blue,
                        translation: new Vector2(-32, -67));
                    t3.GetComponent<TextRendererComponent>().SortingLayerName = Foreground;
                }
            },
            new()
            {
                Name = "Order in layer",
                ExpectedReferenceImageFile = "SortingLayers.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Green, fillInterior: true,
                        translation: new Vector2(-65, 65));
                    var r1 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Red, fillInterior: true,
                        translation: new Vector2(-75, 75));
                    r1.GetComponent<RectangleRendererComponent>().OrderInLayer = -1;
                    var r3 = entityFactory.CreateRectangle(scene, new Vector2(30, 30), Color.Blue, fillInterior: true,
                        translation: new Vector2(-55, 55));
                    r3.GetComponent<RectangleRendererComponent>().OrderInLayer = 1;

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 15, 15, Color.Green, fillInterior: true,
                        translation: new Vector2(65, 65));
                    var e1 = entityFactory.CreateEllipse(scene, 15, 15, Color.Red, fillInterior: true,
                        translation: new Vector2(75, 75));
                    e1.GetComponent<EllipseRendererComponent>().OrderInLayer = -1;
                    var e3 = entityFactory.CreateEllipse(scene, 15, 15, Color.Blue, fillInterior: true,
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
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Green, translation: new Vector2(-31, -66));
                    var t1 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Red,
                        translation: new Vector2(-30, -65));
                    t1.GetComponent<TextRendererComponent>().OrderInLayer = -1;
                    var t3 = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Blue,
                        translation: new Vector2(-32, -67));
                    t3.GetComponent<TextRendererComponent>().OrderInLayer = 1;
                }
            },
            new()
            {
                Name = "Camera transformation",
                ExpectedReferenceImageFile = "CameraTransformation.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    var camera = entityFactory.CreateCamera(scene);
                    var cameraTransform = camera.GetComponent<Transform2DComponent>();
                    cameraTransform.Translation = new Vector2(100, 100);
                    cameraTransform.Rotation = Angle.Deg2Rad(-45);
                    cameraTransform.Scale = new Vector2(3, 3);

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(60, 30), Color.Red, fillInterior: true,
                        translation: new Vector2(-75, 75));

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 30, 15, Color.Green, fillInterior: true,
                        translation: new Vector2(75, 75));

                    // Sprite
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite);

                    // Text
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Blue, translation: new Vector2(-31, -66));
                }
            },
            new()
            {
                Name = "Camera overscan",
                ExpectedReferenceImageFile = "CameraOverscan.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    var camera = entityFactory.CreateCamera(scene);
                    var cameraComponent = camera.GetComponent<CameraComponent>();
                    cameraComponent.ViewRectangle = new Vector2(640, 480);
                    cameraComponent.AspectRatioBehavior = AspectRatioBehavior.Overscan;

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(60, 30), Color.Red, fillInterior: true,
                        translation: new Vector2(-75, 75));

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 30, 15, Color.Green, fillInterior: true,
                        translation: new Vector2(75, 75));

                    // Sprite
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite);

                    // Text
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Blue, translation: new Vector2(-31, -66));
                }
            },
            new()
            {
                Name = "Camera underscan",
                ExpectedReferenceImageFile = "CameraUnderscan.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    var camera = entityFactory.CreateCamera(scene);
                    var cameraComponent = camera.GetComponent<CameraComponent>();
                    cameraComponent.ViewRectangle = new Vector2(640, 480);
                    cameraComponent.AspectRatioBehavior = AspectRatioBehavior.Underscan;

                    // Rectangle
                    entityFactory.CreateRectangle(scene, new Vector2(60, 30), Color.Red, fillInterior: true,
                        translation: new Vector2(-75, 75));

                    // Ellipse
                    entityFactory.CreateEllipse(scene, 30, 15, Color.Green, fillInterior: true,
                        translation: new Vector2(75, 75));

                    // Sprite
                    entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite);

                    // Text
                    entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.Blue, translation: new Vector2(-31, -66));
                }
            },
            new()
            {
                Name = "Transform hierarchy",
                ExpectedReferenceImageFile = "TransformHierarchy.png",
                SetUpScene = (scene, entityFactory, _) =>
                {
                    entityFactory.CreateCamera(scene);

                    CreatePortrait(scene, entityFactory, new Vector2(0, 0), 0, new Vector2(0.5, 0.5));
                    CreatePortrait(scene, entityFactory, new Vector2(-60, 60), Angle.Deg2Rad(45), new Vector2(0.3, 0.3));
                    CreatePortrait(scene, entityFactory, new Vector2(60, -60), Angle.Deg2Rad(-45), new Vector2(0.4, 0.4));

                    static void CreatePortrait(Scene scene, EntityFactory entityFactory, Vector2 translation, double rotation, Vector2 scale)
                    {
                        var root = scene.CreateEntity();
                        var transform2DComponent = root.CreateComponent<Transform2DComponent>();
                        transform2DComponent.Translation = translation;
                        transform2DComponent.Rotation = rotation;
                        transform2DComponent.Scale = scale;

                        var rectangle = entityFactory.CreateRectangle(scene, new Vector2(125, 125), Color.Black, fillInterior: true,
                            rotation: Angle.Deg2Rad(-45));
                        rectangle.GetComponent<RectangleRendererComponent>().OrderInLayer = -2;
                        rectangle.Parent = root;

                        var ellipse = entityFactory.CreateEllipse(scene, 75, 75, Color.FromArgb(255, 100, 100, 100), fillInterior: true);
                        ellipse.GetComponent<EllipseRendererComponent>().OrderInLayer = -1;
                        ellipse.Parent = root;

                        var sprite = entityFactory.CreateSprite(scene, AssetsIds.Sprites.Sample01);
                        sprite.GetComponent<SpriteRendererComponent>().OrderInLayer = 2;
                        sprite.Parent = root;

                        var colorBackground = entityFactory.CreateSprite(scene, AssetsIds.SpriteSheet.FullSprite, scale: new Vector2(1.28, 1.28));
                        colorBackground.GetComponent<SpriteRendererComponent>().OrderInLayer = 0;
                        colorBackground.Parent = root;

                        for (var i = 0; i < 4; i++)
                        {
                            const int x = -49;
                            var y = 48 - i * 25;
                            var watermark = entityFactory.CreateText(scene, "Geisha", FontSize.FromDips(20), Color.FromArgb(100, 255, 255, 255),
                                translation: new Vector2(x, y), scale: new Vector2(1.5, 1.0));
                            watermark.GetComponent<TextRendererComponent>().OrderInLayer = 1;
                            watermark.Parent = colorBackground;
                        }
                    }
                }
            },
            new()
            {
                Name = "Debug renderer",
                ExpectedReferenceImageFile = "DebugRenderer.png",
                SetUpScene = (scene, entityFactory, debugRenderer) =>
                {
                    entityFactory.CreateCamera(scene);

                    debugRenderer.DrawRectangle(new AxisAlignedRectangle(60, 30), Color.Red, Matrix3x3.Identity);
                    debugRenderer.DrawRectangle(new AxisAlignedRectangle(50, 25), Color.Green, Matrix3x3.Identity);
                    debugRenderer.DrawRectangle(new AxisAlignedRectangle(40, 20), Color.Blue, Matrix3x3.Identity);

                    var rectangleTransform =
                        Matrix3x3.CreateTranslation(new Vector2(-50, 50))
                        * Matrix3x3.CreateRotation(Angle.Deg2Rad(45))
                        * Matrix3x3.CreateScale(new Vector2(0.5, 0.5));
                    debugRenderer.DrawRectangle(new AxisAlignedRectangle(60, 30), Color.Red, rectangleTransform);
                    debugRenderer.DrawRectangle(new AxisAlignedRectangle(50, 25), Color.Green, rectangleTransform);
                    debugRenderer.DrawRectangle(new AxisAlignedRectangle(40, 20), Color.Blue, rectangleTransform);

                    debugRenderer.DrawCircle(new Circle(new Vector2(50, -50), 30), Color.Red);
                    debugRenderer.DrawCircle(new Circle(new Vector2(50, -50), 20), Color.Green);
                    debugRenderer.DrawCircle(new Circle(new Vector2(50, -50), 10), Color.Blue);
                }
            }
        };

        [TestCaseSource(nameof(RenderingTestCases))]
        public void RenderScene_ShouldRenderImageSameAsReferenceImage(RenderingTestCase testCase)
        {
            // Arrange
            var scene = SystemUnderTest.SceneManager.CurrentScene;
            var entityFactory = new EntityFactory(SystemUnderTest.AssetStore);

            testCase.SetUpScene(scene, entityFactory, SystemUnderTest.DebugRenderer);

            // Act
            SystemUnderTest.RenderingSystem.RenderScene();

            // Assert
            using var memoryStream = new MemoryStream();
            SystemUnderTest.RenderingBackend.Context2D.CaptureScreenShotAsPng(memoryStream);
            using var actualImage = Image.Load<Bgra32>(memoryStream.ToArray());

            if (SaveRenderedImages)
#pragma warning disable CS0162
                // ReSharper disable HeuristicUnreachableCode
            {
                var testOutputDirectory = Utils.GetPathUnderTestDirectory(Path.Combine("Rendering", "TestOutput"));
                Directory.CreateDirectory(testOutputDirectory);
                var outputImageFilePath = Path.Combine(testOutputDirectory, testCase.ExpectedReferenceImageFile);
                File.WriteAllBytes(outputImageFilePath, memoryStream.ToArray());
            }
            // ReSharper restore HeuristicUnreachableCode
#pragma warning restore CS0162


            var referenceImageFilePath = Utils.GetPathUnderTestDirectory(Path.Combine("Rendering", "ReferenceImages", testCase.ExpectedReferenceImageFile));
            using var referenceImage = Image.Load<Bgra32>(referenceImageFilePath);

            Assert.That(actualImage.Width, Is.EqualTo(referenceImage.Width));
            Assert.That(actualImage.Height, Is.EqualTo(referenceImage.Height));

            for (var y = 0; y < referenceImage.Height; y++)
            {
                for (var x = 0; x < referenceImage.Width; x++)
                {
                    Assert.That(actualImage[x, y], Is.EqualTo(referenceImage[x, y]), $"Images differ at (x,y) = ({x},{y}).");
                }
            }
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
                var entity = scene.CreateEntity();
                entity.CreateComponent<Transform2DComponent>();
                var cameraComponent = entity.CreateComponent<CameraComponent>();
                cameraComponent.ViewRectangle = new Vector2(200, 200);

                return entity;
            }

            public Entity CreateRectangle(Scene scene, Vector2 dimension, Color color, bool fillInterior = false, Vector2? translation = null,
                double rotation = 0, Vector2? scale = null)
            {
                var entity = scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = translation ?? Vector2.Zero;
                transform2DComponent.Rotation = rotation;
                transform2DComponent.Scale = scale ?? Vector2.One;

                var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
                rectangleRendererComponent.Dimension = dimension;
                rectangleRendererComponent.Color = color;
                rectangleRendererComponent.FillInterior = fillInterior;

                return entity;
            }

            public Entity CreateEllipse(Scene scene, double radiusX, double radiusY, Color color, bool fillInterior = false, Vector2? translation = null,
                double rotation = 0, Vector2? scale = null)
            {
                var entity = scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = translation ?? Vector2.Zero;
                transform2DComponent.Rotation = rotation;
                transform2DComponent.Scale = scale ?? Vector2.One;

                var ellipseRendererComponent = entity.CreateComponent<EllipseRendererComponent>();
                ellipseRendererComponent.RadiusX = radiusX;
                ellipseRendererComponent.RadiusY = radiusY;
                ellipseRendererComponent.Color = color;
                ellipseRendererComponent.FillInterior = fillInterior;

                return entity;
            }

            public Entity CreateSprite(Scene scene, AssetId spriteAssetId, Vector2? translation = null, double rotation = 0, Vector2? scale = null)
            {
                var entity = scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = translation ?? Vector2.Zero;
                transform2DComponent.Rotation = rotation;
                transform2DComponent.Scale = scale ?? Vector2.One;

                var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
                spriteRendererComponent.Sprite = _assetStore.GetAsset<Sprite>(spriteAssetId);

                return entity;
            }

            public Entity CreateText(Scene scene, string text, FontSize fontSize, Color color, Vector2? translation = null, double rotation = 0,
                Vector2? scale = null)
            {
                var entity = scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = translation ?? Vector2.Zero;
                transform2DComponent.Rotation = rotation;
                transform2DComponent.Scale = scale ?? Vector2.One;

                var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
                textRendererComponent.Text = text;
                textRendererComponent.FontSize = fontSize;
                textRendererComponent.Color = color;

                return entity;
            }
        }
    }
}