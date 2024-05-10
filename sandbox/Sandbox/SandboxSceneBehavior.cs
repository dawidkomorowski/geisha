using System;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Physics.Components;
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

                var entityController = CreateEntityController();

                //var entity = CreateCircleKinematicBody(0, 0);
                //var entity = CreateSquareKinematicBody(-300, 0);
                //entityController.ControlledEntity = entity;

                //CreateSquareKinematicBody(0, 300);

                //CreateRectangleStaticBody(0, -200, 100, 100);
                //CreateRectangleStaticBody(-300, -300, 200, 100);
                //CreateRectangleStaticBody(-600, -300, 50, 100);
                //CreateRectangleStaticBody(-200, 300, 100, 100);
                //CreateRectangleStaticBody(-300, 200, 100, 100);
                //CreateCircleStaticBody(200, 0);
                //CreateCircleStaticBody(350, 0);
                //CreateCircleStaticBody(450, 0);

                //CreateRectangleStaticBody(200, -300, 100, 100);
                //CreateRectangleStaticBody(300, -300, 100, 100);
                //CreateRectangleStaticBody(400, -300, 100, 100);

                // For unit tests
                CreateRectangleForTests(3, 2, 4, 2, 0);
                CreateCircleForTests(3, -1, 1);

                var random = new Random(0);

                //for (int i = 0; i < 5000; i++)
                //{
                //    double x = random.Next(-800, 800);
                //    double y = random.Next(-450, 450);

                //    CreateBoxSprite(x, y);
                //}

                //for (int i = 0; i < 5000; i++)
                //{
                //    double x = random.Next(-800, 800);
                //    double y = random.Next(-450, 450);

                //    CreateCompassSprite(x, y);
                //}
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

            private Entity CreateRectangleStaticBody(double x, double y, double w, double h)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(x, y);
                var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
                rectangleColliderComponent.Dimensions = new Vector2(w, h);

                return entity;
            }

            private Entity CreateSquareKinematicBody(double x, double y)
            {
                var entity = CreateRectangleStaticBody(x, y, 100, 100);
                entity.CreateComponent<KinematicRigidBody2DComponent>();

                return entity;
            }

            private Entity CreateCircleStaticBody(double x, double y)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(x, y);
                var rectangleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
                rectangleColliderComponent.Radius = 50;

                return entity;
            }

            private Entity CreateCircleKinematicBody(double x, double y)
            {
                var entity = CreateCircleStaticBody(x, y);
                entity.CreateComponent<KinematicRigidBody2DComponent>();

                return entity;
            }

            private void CreateBoxSprite(double x, double y)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Scale = new Vector2(0.1, 0.1);
                transform2DComponent.Translation = new Vector2(x, y);
                var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
                spriteRendererComponent.Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.BoxSprite);
                spriteRendererComponent.OrderInLayer = 1;
            }

            private void CreateCompassSprite(double x, double y)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Scale = new Vector2(0.1, 0.1);
                transform2DComponent.Translation = new Vector2(x, y);
                var spriteRendererComponent = entity.CreateComponent<SpriteRendererComponent>();
                spriteRendererComponent.Sprite = _assetStore.GetAsset<Sprite>(AssetsIds.CompassSprite);
                spriteRendererComponent.OrderInLayer = 2;
            }

            private EntityControllerComponent CreateEntityController()
            {
                var entity = Scene.CreateEntity();
                entity.CreateComponent<InputComponent>();
                return entity.CreateComponent<EntityControllerComponent>();
            }

            private void CreateRectangleForTests(double x, double y, double w, double h, double rotation)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();

                const double scale = 50;

                transform2DComponent.Translation = new Vector2(x, y) * scale;
                transform2DComponent.Rotation = Angle.Deg2Rad(rotation);
                rectangleRendererComponent.Dimensions = new Vector2(w, h) * scale;
                rectangleRendererComponent.Color = Color.Black;
            }

            private void CreateCircleForTests(double x, double y, double r)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                var ellipseRendererComponent = entity.CreateComponent<EllipseRendererComponent>();

                const double scale = 50;

                transform2DComponent.Translation = new Vector2(x, y) * scale;
                ellipseRendererComponent.Radius = r * scale;
                ellipseRendererComponent.Color = Color.Black;
            }
        }
    }
}