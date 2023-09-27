using System;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Sandbox
{
    public sealed class SandboxSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "SandboxSceneBehavior";
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;

        public SandboxSceneBehaviorFactory(IAssetStore assetStore, IEngineManager engineManager, ISceneLoader sceneLoader, ISceneManager sceneManager)
        {
            _assetStore = assetStore;
            _engineManager = engineManager;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) =>
            new SandboxSceneBehavior(
                scene,
                _assetStore,
                _engineManager,
                _sceneLoader,
                _sceneManager
            );

        private sealed class SandboxSceneBehavior : SceneBehavior
        {
            private readonly IAssetStore _assetStore;
            private readonly IEngineManager _engineManager;
            private readonly ISceneLoader _sceneLoader;
            private readonly ISceneManager _sceneManager;

            public SandboxSceneBehavior(Scene scene, IAssetStore assetStore, IEngineManager engineManager, ISceneLoader sceneLoader,
                ISceneManager sceneManager) : base(scene)
            {
                _assetStore = assetStore;
                _engineManager = engineManager;
                _sceneLoader = sceneLoader;
                _sceneManager = sceneManager;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                if (!IsLevelLoadedFromSave())
                {
                    SetUpNewLevel();
                }

                BindBasicControls();
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
            }

            private void BindBasicControls()
            {
                var inputComponent = Scene.AllEntities.Single(e => e.Name == "Controls").GetComponent<InputComponent>();

                inputComponent.BindAction("Exit", _engineManager.ScheduleEngineShutdown);
                inputComponent.BindAction("QuickSave", () => _sceneLoader.Save(_sceneManager.CurrentScene, "quicksave.geisha-scene"));
                inputComponent.BindAction("QuickLoad", () => _sceneManager.LoadScene("quicksave.geisha-scene"));
            }

            private void CreateBasicControls()
            {
                var entity = Scene.CreateEntity();
                entity.Name = "Controls";
                var inputComponent = entity.CreateComponent<InputComponent>();

                inputComponent.InputMapping = _assetStore.GetAsset<InputMapping>(new AssetId(new Guid("4D5E957B-6176-4FFA-966D-5C3403909D9A")));
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
                var layoutRectangle = textRendererComponent.LayoutRectangle;
                var textRectangle = textRendererComponent.TextRectangle;

                var layoutRect = entity.CreateChildEntity();
                layoutRect.CreateComponent<Transform2DComponent>().Translation = layoutRectangle.Center;
                var layoutRectangleRenderer = layoutRect.CreateComponent<RectangleRendererComponent>();
                layoutRectangleRenderer.OrderInLayer = -1;
                layoutRectangleRenderer.Dimension = layoutRectangle.Dimensions;
                layoutRectangleRenderer.Color = Color.Red;

                var textRect = entity.CreateChildEntity();
                textRect.CreateComponent<Transform2DComponent>().Translation = textRectangle.Center;
                var textRectangleRenderer = textRect.CreateComponent<RectangleRendererComponent>();
                textRectangleRenderer.OrderInLayer = -1;
                textRectangleRenderer.Dimension = textRectangle.Dimensions;
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