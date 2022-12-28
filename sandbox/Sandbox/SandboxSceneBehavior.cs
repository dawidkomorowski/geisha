using System;
using System.Linq;
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

        public SandboxSceneBehaviorFactory(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new SandboxSceneBehavior(scene, _assetStore);

        private sealed class SandboxSceneBehavior : SceneBehavior
        {
            private readonly IAssetStore _assetStore;

            public SandboxSceneBehavior(Scene scene, IAssetStore assetStore) : base(scene)
            {
                _assetStore = assetStore;
            }

            public override string Name => SceneBehaviorName;

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
                CreateLerpRectangle(500, -500, 600, -400, 100, 50);

                CreateBox();
                CreateCompass();
                CreateMouseInfoText();
                CreateKeyText();
                CreateCamera();
                CreateMousePointer();
            }

            private void CreateBox()
            {
                var box = Scene.CreateEntity();

                var boxTransform = box.CreateComponent<Transform2DComponent>();
                boxTransform.Translation = new Vector2(200, -400);
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
            }

            private void CreateCompass()
            {
                var compass = Scene.CreateEntity();
                compass.Name = "Compass";

                var transform = compass.CreateComponent<Transform2DComponent>();
                transform.Scale = new Vector2(0.5, 0.5);

                var spriteRenderer = compass.CreateComponent<SpriteRendererComponent>();
                spriteRenderer.Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite);
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

            private void CreateCamera()
            {
                var camera = Scene.CreateEntity();
                camera.CreateComponent<Transform2DComponent>();

                var cameraComponent = camera.CreateComponent<CameraComponent>();
                cameraComponent.ViewRectangle = new Vector2(1600, 900);
                cameraComponent.AspectRatioBehavior = AspectRatioBehavior.Underscan;

                camera.CreateComponent<TopDownCameraForBoxComponent>();
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

            private void CreateLerpRectangle(double x0, double y0, double x1, double y1, double w, double h)
            {
                var rectangle = Scene.CreateEntity();

                rectangle.CreateComponent<Transform2DComponent>();

                var rectangleRenderer = rectangle.CreateComponent<RectangleRendererComponent>();
                rectangleRenderer.Dimension = new Vector2(w, h);
                rectangleRenderer.Color = Color.FromArgb(255, 0, 0, 255);

                var lerpComponent = rectangle.CreateComponent<LerpComponent>();
                lerpComponent.StartPosition = new Vector2(x0, y0);
                lerpComponent.EndPosition = new Vector2(x1, y1);
                lerpComponent.Duration = TimeSpan.FromSeconds(1);
            }
        }
    }
}