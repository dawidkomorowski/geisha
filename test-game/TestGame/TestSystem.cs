using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using TestGame.Behaviors;

namespace TestGame
{
    // TODO Systems should only iterate over entities of interest - some event based (component added/removed etc.) internal list of entities (of interest) should be introduced?
    public sealed class TestSystem : ICustomSystem
    {
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly IAudioPlayer _audioPlayer;
        private readonly IDebugRenderer _debugRenderer;

        public TestSystem(IAssetStore assetStore, IEngineManager engineManager, ISceneLoader sceneLoader, ISceneManager sceneManager,
            IAudioBackend audioBackend, IDebugRenderer debugRenderer)
        {
            _assetStore = assetStore;
            _engineManager = engineManager;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
            _audioPlayer = audioBackend.AudioPlayer;
            _debugRenderer = debugRenderer;
        }

        public string Name => nameof(TestSystem);

        public void ProcessFixedUpdate(Scene scene)
        {
            var box = scene.AllEntities.Single(e => e.HasComponent<BoxMovementComponent>());
            var mousePointer = scene.RootEntities.Single(e => e.HasComponent<MousePointerComponent>());
            var camera = scene.AllEntities.Single(e => e.HasComponent<CameraComponent>());

            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.HasComponent<DieFromBoxComponent>())
                {
                    var collider = entity.GetComponent<CircleColliderComponent>();
                    if (collider.IsColliding)
                    {
                        var collidedWithBox = collider.CollidingEntities.Contains(box);
                        var collidedWithMousePointer = collider.CollidingEntities.Contains(mousePointer);
                        var mousePointerHasLeftButtonPressed = mousePointer.GetComponent<MousePointerComponent>().LeftButtonPressed;

                        if (collidedWithBox || (collidedWithMousePointer && mousePointerHasLeftButtonPressed))
                        {
                            _audioPlayer.PlayOnce(_assetStore.GetAsset<ISound>(AssetsIds.SfxSound));
                            entity.DestroyAfterFixedTimeStep();
                        }
                    }
                }

                if (entity.HasComponent<CloseGameOnEscapeKeyComponent>())
                {
                    var inputComponent = entity.GetComponent<InputComponent>();
                    if (inputComponent.HardwareInput.KeyboardInput.Escape) _engineManager.ScheduleEngineShutdown();
                    if (inputComponent.HardwareInput.KeyboardInput.F5) _sceneLoader.Save(scene, "quicksave.scene");
                    if (inputComponent.HardwareInput.KeyboardInput.F9) _sceneManager.LoadScene("quicksave.scene");

                    var mouseScrollDelta = inputComponent.HardwareInput.MouseInput.ScrollDelta;
                    var scalingFactor = 0.0001 * mouseScrollDelta;
                    //box.GetComponent<Transform2DComponent>().Scale += new Vector2(scalingFactor, scalingFactor);
                    scalingFactor = mouseScrollDelta == 0 ? 1 : mouseScrollDelta > 0 ? 10d / 11d : 11d / 10d;
                    camera.GetComponent<CameraComponent>().ViewRectangle *= scalingFactor;
                }
            }
        }

        public void ProcessUpdate(Scene scene, GameTime gameTime)
        {
            var compass = scene.AllEntities.Single(e => e.Name == "Compass");
            var compassCenter = compass.GetComponent<Transform2DComponent>().Translation;
            _debugRenderer.DrawCircle(new Circle(compassCenter, 100), Color.FromArgb(255, 0, 255, 0));
        }
    }
}