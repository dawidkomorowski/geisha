using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
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
                transform2DComponent.Translation = new Vector2(0, -0);
                var textRendererComponent = entity.CreateComponent<TextRendererComponent>();
                textRendererComponent.Text =
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse eu velit augue. Phasellus dictum erat metus, at pharetra tortor sagittis in. Quisque sapien metus, varius ac blandit ac, fermentum eget risus. Nunc lacus tellus, vulputate a laoreet et, suscipit ut dolor. Etiam a ante sit amet arcu dignissim venenatis ac at magna. Phasellus rhoncus neque mollis ante fermentum luctus. Integer ante ex, tempus a rhoncus et, vehicula non massa.";
            }
        }
    }
}