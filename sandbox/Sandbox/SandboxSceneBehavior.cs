using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Sandbox.Behaviors;

namespace Sandbox
{
    public sealed class SandboxSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "SandboxSceneBehavior";
        private readonly IAssetStore _assetStore;
        private readonly IAudioBackend _audioBackend;

        public SandboxSceneBehaviorFactory(IAssetStore assetStore, IAudioBackend audioBackend)
        {
            _assetStore = assetStore;
            _audioBackend = audioBackend;
        }

        public string BehaviorName { get; } = SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new SandboxSceneBehavior(scene, _assetStore, _audioBackend);

        // TODO Add API to enable/disable sound globally?
        private sealed class SandboxSceneBehavior : SceneBehavior
        {
            private readonly IAssetStore _assetStore;
            private readonly IAudioPlayer _audioPlayer;

            public SandboxSceneBehavior(Scene scene, IAssetStore assetStore, IAudioBackend audioBackend) : base(scene)
            {
                _assetStore = assetStore;
                _audioPlayer = audioBackend.AudioPlayer;
            }

            public override string Name { get; } = SceneBehaviorName;

            protected override void OnLoaded()
            {
                if (!IsLevelLoadedFromSave())
                {
                    SetUpNewLevel();
                }
            }

            private bool IsLevelLoadedFromSave()
            {
                return Scene.AllEntities.Any(e => e.HasComponent<BoxMovementComponent>());
            }

            private void SetUpNewLevel()
            {
                var random = new Random();

                for (var i = 0; i < 100; i++)
                {
                    CreateDot(-500 + random.Next(1000), -350 + random.Next(700));
                }

                CreateRectangle(1000, 10, 400, 200);
                CreateRectangle(600, -100, 300, 100, false);
                CreateRectangle(300, -600, 1600, 900, false);
                CreateEllipse(-1000, 10, 200, 100);
                CreateEllipse(-600, -100, 150, 50, false);
                CreateBox();
                CreateCompass();
                CreateRotatingText();
                CreateMouseInfoText();
                CreateKeyText();
                CreateCamera();
                CreateBackgroundMusic();
                CreateMousePointer();

                for (var i = 0; i < 100; i++)
                {
                    CreateCampfireAnimation(-500 + random.Next(1000), -350 + random.Next(700));
                }
            }

            private void CreateSimpleDot(double x, double y)
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

                Scene.AddEntity(dot);
            }

            private void CreateDot(double x, double y)
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

                Scene.AddEntity(dot);
            }

            private void CreateBox()
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

                Scene.AddEntity(box);
            }

            private void CreateCompass()
            {
                var compass = new Entity {Name = "Compass"};
                compass.AddComponent(new Transform2DComponent
                {
                    Translation = Vector2.Zero,
                    Rotation = 0,
                    Scale = new Vector2(0.5, 0.5)
                });
                compass.AddComponent(new SpriteRendererComponent {Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite)});
                compass.AddComponent(new RotateComponent());
                compass.AddComponent(new FollowEllipseComponent {Velocity = 2, Width = 100, Height = 100});

                Scene.AddEntity(compass);
            }

            private void CreateRotatingText()
            {
                var text = new Entity();
                text.AddComponent(Transform2DComponent.CreateDefault());
                text.AddComponent(new TextRendererComponent {Text = "I am Text!", Color = Color.FromArgb(255, 0, 255, 0), FontSize = FontSize.FromPoints(16)});
                text.AddComponent(new FollowEllipseComponent {Velocity = 1, Width = 300, Height = 300});
                text.AddComponent(new RotateComponent());
                text.AddComponent(new DoMagicWithTextComponent());

                Scene.AddEntity(text);
            }

            private void CreateMouseInfoText()
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

                Scene.AddEntity(text);
            }

            private void CreateKeyText()
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

                Scene.AddEntity(text);
            }

            private void CreateRectangle(double x, double y, double w, double h, bool fillInterior = true)
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

                Scene.AddEntity(rectangle);
            }

            private void CreateEllipse(double x, double y, double radiusX, double radiusY, bool fillInterior = true)
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

                Scene.AddEntity(rectangle);
            }

            private void CreateCamera()
            {
                var camera = new Entity();
                camera.AddComponent(new Transform2DComponent
                {
                    Translation = new Vector2(0, 0),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                camera.AddComponent(new CameraComponent {ViewRectangle = new Vector2(1600, 900), AspectRatioBehavior = AspectRatioBehavior.Underscan});
                camera.AddComponent(new TopDownCameraForBoxComponent());

                Scene.AddEntity(camera);
            }

            private void CreateBackgroundMusic()
            {
                var music = new Entity();
                music.AddComponent(new InputComponent());

                var playback = _audioPlayer.Play(_assetStore.GetAsset<ISound>(AssetsIds.MusicSound));
                music.AddComponent(new MusicControllerComponent {Playback = playback});
                Scene.AddEntity(music);
            }

            private void CreateMousePointer()
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

                Scene.AddEntity(mousePointer);
            }

            private void CreateCampfireAnimation(double x, double y)
            {
                var campfire = new Entity();
                campfire.AddComponent(new Transform2DComponent
                {
                    Translation = new Vector2(x, y),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                campfire.AddComponent(new SpriteRendererComponent());
                var spriteAnimationComponent = new SpriteAnimationComponent();
                campfire.AddComponent(spriteAnimationComponent);

                spriteAnimationComponent.AddAnimation("main", _assetStore.GetAsset<SpriteAnimation>(AssetsIds.CampfireAnimation));
                spriteAnimationComponent.PlayAnimation("main");
                spriteAnimationComponent.PlayInLoop = true;

                var random = new Random();
                spriteAnimationComponent.Position = random.NextDouble();

                Scene.AddEntity(campfire);
            }
        }
    }
}