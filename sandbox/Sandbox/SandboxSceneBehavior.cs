using System;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

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
                return Scene.AllEntities.Any();
            }

            private void SetUpNewLevel()
            {
                CreateBasicControls();
                CreateCamera();
                CreatePoint(0, 0);
                CreateText();
                CreateChangingText();
            }

            private void CreateBasicControls()
            {
                var entity = Scene.CreateEntity();
                entity.CreateComponent<InputComponent>();
            }

            private void CreateCamera()
            {
                var camera = Scene.CreateEntity();
                camera.CreateComponent<Transform2DComponent>();

                var cameraComponent = camera.CreateComponent<CameraComponent>();
                cameraComponent.ViewRectangle = new Vector2(1600, 900);
                cameraComponent.AspectRatioBehavior = AspectRatioBehavior.Underscan;
            }

            private void CreatePoint(int x, int y)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(x, y);
                var ellipseRendererComponent = entity.CreateComponent<EllipseRendererComponent>();
                ellipseRendererComponent.FillInterior = true;
                ellipseRendererComponent.Radius = 2;
                ellipseRendererComponent.Color = Color.Red;
            }

            private void CreateText()
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Rotation = Angle.Deg2Rad(45);
                transform2DComponent.Scale = new Vector2(0.5, 0.5);

                var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
                textRendererComponent.Text =
                    @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla malesuada pharetra mattis. Donec finibus id mi sed congue. Aenean scelerisque a nulla et sollicitudin. Quisque vitae neque laoreet, dapibus mi sed, vulputate ipsum. Sed varius turpis at erat consequat venenatis. Vivamus id risus sed diam auctor feugiat. Suspendisse sodales sem sit amet elementum cursus. Quisque sit amet elementum nibh, vel elementum dolor. Cras ultrices nibh erat, nec ultricies nisi sodales vitae. Duis iaculis vestibulum risus id sodales. Etiam tempor nisl eu nunc bibendum, sed sodales turpis pharetra. Mauris auctor sapien orci, in finibus dolor iaculis id. Aenean ornare tellus ut feugiat aliquam.

Nunc luctus imperdiet urna semper mattis. Donec at tortor dignissim neque luctus iaculis. Maecenas condimentum libero quis dolor dictum mollis. Proin in feugiat nulla. Suspendisse tincidunt, mi varius auctor accumsan, tellus urna vehicula magna, a auctor urna tellus non metus. Donec metus odio, pharetra nec elit et, molestie tincidunt lorem. Cras blandit nibh sodales varius gravida. Suspendisse facilisis porta ipsum, ut lobortis purus vestibulum vitae. Duis rutrum eros ac nulla varius, nec ultricies elit ultricies. Integer et mi dolor. Vestibulum efficitur diam ullamcorper, finibus libero et, fringilla lectus. Donec bibendum sem quam, vel consectetur elit efficitur quis. Quisque in nibh at massa pellentesque convallis. Curabitur mattis rutrum ligula, id varius mi pharetra vitae. Integer quis ultrices risus. ";
                textRendererComponent.MaxWidth = 700;
                textRendererComponent.MaxHeight = 1000;
                textRendererComponent.TextAlignment = TextAlignment.Justified;
                textRendererComponent.Pivot = new Vector2(200, 100);

                var layoutRect = entity.CreateChildEntity();
                layoutRect.CreateComponent<Transform2DComponent>();
                var layoutRectangleRenderer = layoutRect.CreateComponent<RectangleRendererComponent>();
                layoutRectangleRenderer.OrderInLayer = -1;
                layoutRectangleRenderer.Dimension = new Vector2(100, 100);
                layoutRectangleRenderer.Color = Color.Red;

                var textRect = entity.CreateChildEntity();
                textRect.CreateComponent<Transform2DComponent>();
                var textRectangleRenderer = textRect.CreateComponent<RectangleRendererComponent>();
                textRectangleRenderer.OrderInLayer = -1;
                textRectangleRenderer.Dimension = new Vector2(200, 200);
                textRectangleRenderer.Color = Color.Blue;
            }

            private void CreateChangingText()
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(-200, 200);
                var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
                textRendererComponent.Color = Color.Red;
                textRendererComponent.TextAlignment = TextAlignment.Center;
                textRendererComponent.ParagraphAlignment = ParagraphAlignment.Center;
                textRendererComponent.MaxWidth = 500;
                textRendererComponent.MaxHeight = 500;
                textRendererComponent.Pivot = new Vector2(250, 250);
                entity.CreateComponent<ChangingTextComponent>();
            }
        }
    }
}