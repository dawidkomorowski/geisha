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
using Geisha.TestGame.Behaviors;

namespace Geisha.TestGame
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
            CreateText(scene);
            CreateKeyText(scene);
            CreateCamera(scene);
            CreateBackgroundMusic(scene);
            CreateMousePointer(scene);
        }

        private void CreateSimpleDot(Scene scene, double x, double y)
        {
            var random = new Random();
            var dot = new Entity {Name = "Dot"};
            dot.AddComponent(new TransformComponent
            {
                Scale = Vector3.One
            });
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
            dot.AddComponent(new TransformComponent
            {
                Scale = Vector3.One
            });
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
            box.AddComponent(new TransformComponent
            {
                Translation = new Vector3(300, -600, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 1)
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

            var boxLabel = new Entity {Name = "BoxLabel"};
            boxLabel.AddComponent(TransformComponent.Default);
            boxLabel.AddComponent(new TextRendererComponent
            {
                Text = "I am Box!",
                SortingLayerName = "Box",
                Color = Color.FromArgb(255, 255, 0, 0),
                FontSize = FontSize.FromPoints(24)
            });
            box.AddChild(boxLabel);

            scene.AddEntity(box);
        }

        private void CreateCompass(Scene scene)
        {
            var compass = new Entity();
            compass.AddComponent(new TransformComponent
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(0.5, 0.5, 1)
            });
            compass.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite)});
            compass.AddComponent(new RotateComponent());
            compass.AddComponent(new FollowEllipseComponent {Velocity = 2, Width = 100, Height = 100});

            scene.AddEntity(compass);
        }

        private void CreateText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new TransformComponent
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(1, 1, 1)
            });
            text.AddComponent(new TextRendererComponent {Text = "I am Text!", Color = Color.FromArgb(255, 0, 255, 0), FontSize = FontSize.FromPoints(16)});
            text.AddComponent(new FollowEllipseComponent {Velocity = 1, Width = 300, Height = 300});
            text.AddComponent(new RotateComponent());
            text.AddComponent(new DoMagicWithTextComponent());

            scene.AddEntity(text);
        }

        private void CreateKeyText(Scene scene)
        {
            var text = new Entity();
            text.AddComponent(new TransformComponent
            {
                Translation = Vector3.Zero,
                Rotation = Vector3.Zero,
                Scale = Vector3.One
            });
            text.AddComponent(
                new TextRendererComponent {Text = "No key pressed.", Color = Color.FromArgb(255, 255, 0, 255), FontSize = FontSize.FromPoints(40)});
            text.AddComponent(new InputComponent());
            text.AddComponent(new SetTextForCurrentKeyComponent());

            scene.AddEntity(text);
        }

        private void CreateRectangle(Scene scene, double x, double y, double w, double h, bool fillInterior = true)
        {
            var rectangle = new Entity();
            rectangle.AddComponent(new TransformComponent
            {
                Translation = new Vector3(x, y, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
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
            rectangle.AddComponent(new TransformComponent
            {
                Translation = new Vector3(x, y, 0),
                Rotation = Vector3.Zero,
                Scale = Vector3.One
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
            camera.AddComponent(new TransformComponent
            {
                Translation = new Vector3(0, 0, 0),
                Rotation = new Vector3(0, 0, 0),
                Scale = new Vector3(resolutionScale, resolutionScale, 1)
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
            mousePointer.AddComponent(new TransformComponent
            {
                Scale = Vector3.One
            });
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