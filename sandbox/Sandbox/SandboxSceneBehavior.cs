using System;
using System.Linq;
using Geisha.Engine.Animation;
using Geisha.Engine.Animation.Components;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
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
                    //CreateDot(-500 + random.Next(1000), -350 + random.Next(700));
                }

                CreateLerpRectangle(500, -500, 600, -400, 100, 50);

                //CreateRectangle(1000, 10, 400, 200);
                //CreateRectangle(600, -100, 300, 100, false);
                //CreateRectangle(300, -600, 1600, 900, false);
                //CreateEllipse(-1000, 10, 200, 100);
                //CreateEllipse(-600, -100, 150, 50, false);
                CreateBox();
                CreateCompass();
                CreateRotatingText();
                CreateMouseInfoText();
                CreateKeyText();
                CreateCamera();
                //CreateBackgroundMusic();
                CreateMousePointer();

                for (var i = 0; i < 100; i++)
                {
                    //CreateCampfireAnimation(-500 + random.Next(1000), -350 + random.Next(700));
                }
            }

            private void CreateDot(double x, double y)
            {
                var random = new Random();
                var dot = Scene.CreateEntity();
                dot.Name = "Dot";

                dot.CreateComponent<Transform2DComponent>();

                var ellipseRenderer = dot.CreateComponent<EllipseRendererComponent>();
                ellipseRenderer.Radius = 32;
                ellipseRenderer.Color = Color.FromArgb(255, 0, 0, 0);
                ellipseRenderer.FillInterior = true;

                var followEllipse = dot.CreateComponent<FollowEllipseComponent>();
                followEllipse.Velocity = random.NextDouble() * 2 + 1;
                followEllipse.Width = 10;
                followEllipse.Height = 10;
                followEllipse.X = x;
                followEllipse.Y = y;

                dot.CreateComponent<DieFromBoxComponent>();
                var circleCollider = dot.CreateComponent<CircleColliderComponent>();
                circleCollider.Radius = 32;
            }

            private void CreateBox()
            {
                var box = Scene.CreateEntity();

                var boxTransform = box.CreateComponent<Transform2DComponent>();
                boxTransform.Translation = new Vector2(300, -600);
                boxTransform.Rotation = 0;
                boxTransform.Scale = new Vector2(0.5, 0.5);

                var spriteRenderer = box.CreateComponent<SpriteRendererComponent>();
                spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.BoxSprite);
                spriteRenderer.SortingLayerName = "Box";

                var inputComponent = box.CreateComponent<InputComponent>();
                inputComponent.InputMapping = _assetStore.GetAsset<InputMapping>(AssetsIds.PlayerInput);

                box.CreateComponent<BoxMovementComponent>();

                var rectangleCollider = box.CreateComponent<RectangleColliderComponent>();
                rectangleCollider.Dimensions = new Vector2(512, 512);

                box.CreateComponent<CloseGameOnEscapeKeyComponent>();

                var boxLabel = box.CreateChildEntity();

                var boxLabelTransform = boxLabel.CreateComponent<Transform2DComponent>();
                boxLabelTransform.Translation = new Vector2(-80, 350);

                var textRenderer = boxLabel.CreateComponent<TextRendererComponent>();
                textRenderer.Text = "I am Box!";
                textRenderer.SortingLayerName = "Box";
                textRenderer.Color = Color.FromArgb(255, 255, 0, 0);
                textRenderer.FontSize = FontSize.FromDips(40);

                var boxRect = box.CreateChildEntity();

                var boxRectTransform = boxRect.CreateComponent<Transform2DComponent>();
                boxRectTransform.Translation = new Vector2(-350, 0);

                var rectangleRenderer = boxRect.CreateComponent<RectangleRendererComponent>();
                rectangleRenderer.SortingLayerName = "Box";
                rectangleRenderer.Color = Color.FromArgb(255, 255, 0, 0);
                rectangleRenderer.Dimension = new Vector2(40, 80);
                rectangleRenderer.FillInterior = true;

                var followEllipse = boxRect.CreateComponent<FollowEllipseComponent>();
                followEllipse.Height = 100;
                followEllipse.Width = 100;
                followEllipse.X = -350;
                followEllipse.Y = 0;
            }

            private void CreateCompass()
            {
                var compass = Scene.CreateEntity();
                compass.Name = "Compass";

                var transform = compass.CreateComponent<Transform2DComponent>();
                transform.Scale = new Vector2(0.5, 0.5);

                var spriteRenderer = compass.CreateComponent<SpriteRendererComponent>();
                spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite);

                compass.CreateComponent<RotateComponent>();

                var followEllipse = compass.CreateComponent<FollowEllipseComponent>();
                followEllipse.Velocity = 2;
                followEllipse.Width = 100;
                followEllipse.Height = 100;
            }

            private void CreateRotatingText()
            {
                var text = Scene.CreateEntity();
                text.CreateComponent<Transform2DComponent>();

                var textRenderer = text.CreateComponent<TextRendererComponent>();
                textRenderer.Text = "I am Text!";
                textRenderer.Color = Color.FromArgb(255, 0, 255, 0);
                textRenderer.FontSize = FontSize.FromPoints(16);

                var followEllipse = text.CreateComponent<FollowEllipseComponent>();
                followEllipse.Velocity = 1;
                followEllipse.Width = 300;
                followEllipse.Height = 300;

                text.CreateComponent<RotateComponent>();
                text.CreateComponent<DoMagicWithTextComponent>();
            }

            private void CreateMouseInfoText()
            {
                var text = Scene.CreateEntity();

                var transform = text.CreateComponent<Transform2DComponent>();
                transform.Translation = new Vector2(0, 30);

                var textRenderer = text.CreateComponent<TextRendererComponent>();
                textRenderer.Color = Color.FromArgb(255, 0, 255, 255);
                textRenderer.FontSize = FontSize.FromDips(25);
                textRenderer.SortingLayerName = "UI";

                text.CreateComponent<InputComponent>();
                text.CreateComponent<SetTextForMouseInfoComponent>();
            }

            private void CreateKeyText()
            {
                var text = Scene.CreateEntity();
                text.CreateComponent<Transform2DComponent>();

                var textRenderer = text.CreateComponent<TextRendererComponent>();
                textRenderer.Color = Color.FromArgb(255, 255, 0, 255);
                textRenderer.FontSize = FontSize.FromDips(25);
                textRenderer.SortingLayerName = "UI";

                text.CreateComponent<InputComponent>();
                text.CreateComponent<SetTextForCurrentKeyComponent>();
            }

            private void CreateRectangle(double x, double y, double w, double h, bool fillInterior = true)
            {
                var rectangle = Scene.CreateEntity();

                var transform = rectangle.CreateComponent<Transform2DComponent>();
                transform.Translation = new Vector2(x, y);

                var rectangleRenderer = rectangle.CreateComponent<RectangleRendererComponent>();
                rectangleRenderer.Dimension = new Vector2(w, h);
                rectangleRenderer.Color = Color.FromArgb(255, 0, 0, 255);
                rectangleRenderer.FillInterior = fillInterior;
            }

            private void CreateEllipse(double x, double y, double radiusX, double radiusY, bool fillInterior = true)
            {
                var ellipse = Scene.CreateEntity();

                var transform = ellipse.CreateComponent<Transform2DComponent>();
                transform.Translation = new Vector2(x, y);

                var ellipseRenderer = ellipse.CreateComponent<EllipseRendererComponent>();
                ellipseRenderer.RadiusX = radiusX;
                ellipseRenderer.RadiusY = radiusY;
                ellipseRenderer.Color = Color.FromArgb(255, 0, 0, 255);
                ellipseRenderer.FillInterior = fillInterior;
            }

            private void CreateCamera()
            {
                var camera = Scene.CreateEntity();
                camera.CreateComponent<Transform2DComponent>();

                var cameraComponent = camera.CreateComponent<CameraComponent>();
                cameraComponent.ViewRectangle = new Vector2(1600, 900);
                cameraComponent.AspectRatioBehavior = AspectRatioBehavior.Underscan;

                camera.CreateComponent<TopDownCameraForBoxComponent>();
            }

            private void CreateBackgroundMusic()
            {
                var music = Scene.CreateEntity();
                music.CreateComponent<InputComponent>();

                var musicController = music.CreateComponent<MusicControllerComponent>();
                musicController.Playback = _audioPlayer.Play(_assetStore.GetAsset<ISound>(AssetsIds.MusicSound));
            }

            private void CreateMousePointer()
            {
                var mousePointer = Scene.CreateEntity();
                mousePointer.Name = "MousePointer";
                mousePointer.CreateComponent<Transform2DComponent>();

                var ellipseRenderer = mousePointer.CreateComponent<EllipseRendererComponent>();
                ellipseRenderer.Radius = 10;
                ellipseRenderer.Color = Color.FromArgb(255, 255, 0, 0);
                ellipseRenderer.FillInterior = true;

                var circleCollider = mousePointer.CreateComponent<CircleColliderComponent>();
                circleCollider.Radius = 10;

                mousePointer.CreateComponent<InputComponent>();
                mousePointer.CreateComponent<MousePointerComponent>();
            }

            private void CreateCampfireAnimation(double x, double y)
            {
                var campfire = Scene.CreateEntity();

                var transform = campfire.CreateComponent<Transform2DComponent>();
                transform.Translation = new Vector2(x, y);

                campfire.CreateComponent<SpriteRendererComponent>();

                var spriteAnimationComponent = campfire.CreateComponent<SpriteAnimationComponent>();
                spriteAnimationComponent.AddAnimation("main", _assetStore.GetAsset<SpriteAnimation>(AssetsIds.CampfireAnimation));
                spriteAnimationComponent.PlayAnimation("main");
                spriteAnimationComponent.PlayInLoop = true;

                var random = new Random();
                spriteAnimationComponent.Position = random.NextDouble();
            }

            private void CreateLerpRectangle(double x0, double y0, double x1, double y1, double w, double h, bool fillInterior = true)
            {
                var rectangle = Scene.CreateEntity();

                rectangle.CreateComponent<Transform2DComponent>();

                var rectangleRenderer = rectangle.CreateComponent<RectangleRendererComponent>();
                rectangleRenderer.Dimension = new Vector2(w, h);
                rectangleRenderer.Color = Color.FromArgb(255, 0, 0, 255);
                rectangleRenderer.FillInterior = fillInterior;

                var lerpComponent = rectangle.CreateComponent<LerpComponent>();
                lerpComponent.StartPosition = new Vector2(x0, y0);
                lerpComponent.EndPosition = new Vector2(x1, y1);
                lerpComponent.Duration = TimeSpan.FromSeconds(1);
            }
        }
    }
}