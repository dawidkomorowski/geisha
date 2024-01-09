using System;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
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

                CreateKinematicBody(0, 0);

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

            private void CreateKinematicBody(double x, double y)
            {
                var entity = Scene.CreateEntity();
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(x, y);
                var rectangleRendererComponent = entity.CreateComponent<RectangleRendererComponent>();
                rectangleRendererComponent.Dimensions = new Vector2(50, 50);
                var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
                rectangleColliderComponent.Dimensions = new Vector2(50, 50);
                var kinematicRigidBody2DComponent = entity.CreateComponent<KinematicRigidBody2DComponent>();
                var inputComponent = entity.CreateComponent<InputComponent>();
                inputComponent.InputMapping = new InputMapping
                {
                    AxisMappings =
                    {
                        new AxisMapping
                        {
                            AxisName = "MoveRight",
                            HardwareAxes =
                            {
                                new HardwareAxis
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Right),
                                    Scale = 1
                                },
                                new HardwareAxis
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Left),
                                    Scale = -1
                                }
                            }
                        },
                        new AxisMapping
                        {
                            AxisName = "MoveUp",
                            HardwareAxes =
                            {
                                new HardwareAxis
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Up),
                                    Scale = 1
                                },
                                new HardwareAxis
                                {
                                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Down),
                                    Scale = -1
                                }
                            }
                        }
                    }
                };
                var velocity = new Vector2();
                inputComponent.BindAxis("MoveRight",
                    scale =>
                    {
                        velocity = velocity.WithX(scale);
                        kinematicRigidBody2DComponent.LinearVelocity = velocity.OfLength(250);
                    });
                inputComponent.BindAxis("MoveUp",
                    scale =>
                    {
                        velocity = velocity.WithY(scale);
                        kinematicRigidBody2DComponent.LinearVelocity = velocity.OfLength(250);
                    });
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
        }
    }
}