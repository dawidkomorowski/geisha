using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using TestGame.Behaviors;

namespace TestGame
{
    public class TestConstructionScript : ISceneConstructionScript
    {
        private readonly IAssetStore _assetStore;

        public TestConstructionScript(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public string Name => nameof(TestConstructionScript);

        public void Execute(Scene scene)
        {
            if (!IsLevelLoadedFromSave(scene))
            {
                SetUpNewLevel(scene);
            }
        }

        private bool IsLevelLoadedFromSave(Scene scene)
        {
            return scene.AllEntities.Any(e => e.HasComponent<BoxMovementComponent>());
        }

        private void SetUpNewLevel(Scene scene)
        {
            var random = new Random();

            for (var i = 0; i < 100; i++)
            {
                CreateDot(scene, -500 + random.Next(1000), -350 + random.Next(700));
            }

            CreateRectangle(scene, 1000, 10, 400, 200);
            CreateRectangle(scene, 600, -100, 300, 100, false);
            CreateEllipse(scene, -1000, 10, 200, 100);
            CreateEllipse(scene, -600, -100, 150, 50, false);
            CreateBox(scene);
            CreateCompass(scene);
            CreateRotatingText(scene);
            CreateMouseInfoText(scene);
            CreateKeyText(scene);
            CreateCamera(scene);
            CreateBackgroundMusic(scene);
            CreateMousePointer(scene);
        }

        private void CreateSimpleDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity {Name = "Dot"};
            dot.AddComponent(Transform2DComponent.CreateDefault());
            dot.AddComponent(new EllipseRendererComponent
            {
                Radius = 32,
                Color = Color.FromArgb(255, 0, 0, 0),
                FillInterior = true
            });
            dot.AddComponent(new FollowEllipseComponent
            {
                Velocity = random.NextDouble() * 2 + 1,
                Width = 10,
                Height = 10,
                X = x,
                Y = y
            });

            scene.AddEntity(dot);
        }

        private void CreateDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity {Name = "Dot"};
            dot.AddComponent(Transform2DComponent.CreateDefault());
            dot.AddComponent(new EllipseRendererComponent
            {
                Radius = 32,
                Color = Color.FromArgb(255, 0, 0, 0),
                FillInterior = true
            });
            dot.AddComponent(new FollowEllipseComponent
            {
                Velocity = random.NextDouble() * 2 + 1,
                Width = 10,
                Height = 10,
                X = x,
                Y = y
            });
            dot.AddComponent(new DieFromBoxComponent());
            dot.AddComponent(new CircleColliderComponent {Radius = 32});

            scene.AddEntity(dot);
        }

        private void CreateBox(Scene scene)
        {
            var box = new Entity();
            box.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(300, -600),
                Rotation = 0,
                Scale = new Vector2(0.5, 0.5)
            });
            box.AddComponent(new SpriteRendererComponent
            {
                Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.BoxSprite),
                SortingLayerName = "Box"
            });
            box.AddComponent(new InputComponent {InputMapping = _assetStore.GetAsset<InputMapping>(AssetsIds.PlayerInput)});
            box.AddComponent(new BoxMovementComponent());
            box.AddComponent(new RectangleColliderComponent {Dimension = new Vector2(512, 512)});
            box.AddComponent(new CloseGameOnEscapeKeyComponent());

            var boxLabel = new Entity();
            boxLabel.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(-80, 350),
                Rotation = 0,
                Scale = Vector2.One
            });
            boxLabel.AddComponent(new TextRendererComponent
            {
                Text = "I am Box!",
                SortingLayerName = "Box",
                Color = Color.FromArgb(255, 255, 0, 0),
                FontSize = FontSize.FromDips(40)
            });
            box.AddChild(boxLabel);

            var boxRect = new Entity();
            boxRect.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(-350, 0),
                Rotation = 0,
                Scale = Vector2.One
            });
            boxRect.AddComponent(new RectangleRendererComponent
            {
                SortingLayerName = "Box",
                Color = Color.FromArgb(255, 255, 0, 0),
                Dimension = new Vector2(40, 80),
                FillInterior = true
            });
            boxRect.AddComponent(new FollowEllipseComponent
            {
                Height = 100,
                Width = 100,
                X = -350,
                Y = 0
            });
            box.AddChild(boxRect);

            scene.AddEntity(box);
        }

        private void CreateCompass(Scene scene)
        {
            var compass = new Entity();
            compass.AddComponent(new Transform2DComponent
            {
                Translation = Vector2.Zero,
                Rotation = 0,
                Scale = new Vector2(0.5, 0.5)
            });
            compass.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite)});
            compass.AddComponent(new RotateComponent());
            compass.AddComponent(new FollowEllipseComponent {Velocity = 2, Width = 100, Height = 100});

            scene.AddEntity(compass);
        }

        private void CreateRotatingText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(Transform2DComponent.CreateDefault());
            text.AddComponent(new TextRendererComponent {Text = "I am Text!", Color = Color.FromArgb(255, 0, 255, 0), FontSize = FontSize.FromPoints(16)});
            text.AddComponent(new FollowEllipseComponent {Velocity = 1, Width = 300, Height = 300});
            text.AddComponent(new RotateComponent());
            text.AddComponent(new DoMagicWithTextComponent());

            scene.AddEntity(text);
        }

        private void CreateMouseInfoText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(0, 30),
                Rotation = 0,
                Scale = Vector2.One
            });
            text.AddComponent(new TextRendererComponent
            {
                Color = Color.FromArgb(255, 0, 255, 255),
                FontSize = FontSize.FromDips(25),
                SortingLayerName = "UI"
            });
            text.AddComponent(new InputComponent());
            text.AddComponent(new SetTextForMouseInfoComponent());

            scene.AddEntity(text);
        }

        private void CreateKeyText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(Transform2DComponent.CreateDefault());
            text.AddComponent(new TextRendererComponent
            {
                Color = Color.FromArgb(255, 255, 0, 255),
                FontSize = FontSize.FromDips(25),
                SortingLayerName = "UI"
            });
            text.AddComponent(new InputComponent());
            text.AddComponent(new SetTextForCurrentKeyComponent());

            scene.AddEntity(text);
        }

        private void CreateRectangle(Scene scene, double x, double y, double w, double h, bool fillInterior = true)
        {
            var rectangle = new Entity();
            rectangle.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            rectangle.AddComponent(new RectangleRendererComponent
            {
                Dimension = new Vector2(w, h),
                Color = Color.FromArgb(255, 0, 0, 255),
                FillInterior = fillInterior
            });

            scene.AddEntity(rectangle);
        }

        private void CreateEllipse(Scene scene, double x, double y, double radiusX, double radiusY, bool fillInterior = true)
        {
            var rectangle = new Entity();
            rectangle.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(x, y),
                Rotation = 0,
                Scale = Vector2.One
            });
            rectangle.AddComponent(new EllipseRendererComponent
            {
                RadiusX = radiusX,
                RadiusY = radiusY,
                Color = Color.FromArgb(255, 0, 0, 255),
                FillInterior = fillInterior
            });

            scene.AddEntity(rectangle);
        }

        private void CreateCamera(Scene scene)
        {
            const double resolutionScale = 720d / 720d;

            var camera = new Entity();
            camera.AddComponent(new Transform2DComponent
            {
                Translation = new Vector2(0, 0),
                Rotation = 0,
                Scale = new Vector2(resolutionScale, resolutionScale)
            });
            camera.AddComponent(new CameraComponent());
            camera.AddComponent(new TopDownCameraForBoxComponent());

            scene.AddEntity(camera);
        }

        private void CreateBackgroundMusic(Scene scene)
        {
            var music = new Entity();
            music.AddComponent(new AudioSourceComponent {Sound = _assetStore.GetAsset<ISound>(AssetsIds.MusicSound)});
            scene.AddEntity(music);
        }

        private void CreateMousePointer(Scene scene)
        {
            var mousePointer = new Entity {Name = "MousePointer"};
            mousePointer.AddComponent(Transform2DComponent.CreateDefault());
            mousePointer.AddComponent(new EllipseRendererComponent
            {
                Radius = 10,
                Color = Color.FromArgb(255, 255, 0, 0),
                FillInterior = true
            });
            mousePointer.AddComponent(new CircleColliderComponent {Radius = 10});
            mousePointer.AddComponent(new InputComponent());
            mousePointer.AddComponent(new MousePointerComponent());

            scene.AddEntity(mousePointer);
        }
    }
}