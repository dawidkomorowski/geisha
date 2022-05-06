using System.Linq;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;
using Sandbox.Behaviors;

namespace Sandbox
{
    public sealed class SandboxSystem : ICustomSystem
    {
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly IAudioPlayer _audioPlayer;

        public SandboxSystem(IAssetStore assetStore, IEngineManager engineManager, ISceneLoader sceneLoader, ISceneManager sceneManager,
            IAudioBackend audioBackend)
        {
            _assetStore = assetStore;
            _engineManager = engineManager;
            _sceneLoader = sceneLoader;
            _sceneManager = sceneManager;
            _audioPlayer = audioBackend.AudioPlayer;
        }

        public string Name => nameof(SandboxSystem);

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
                            entity.RemoveAfterFixedTimeStep();
                        }
                    }
                }

                if (entity.HasComponent<CloseGameOnEscapeKeyComponent>())
                {
                    var inputComponent = entity.GetComponent<InputComponent>();
                    if (inputComponent.HardwareInput.KeyboardInput.Escape) _engineManager.ScheduleEngineShutdown();
                    if (inputComponent.HardwareInput.KeyboardInput.F5) _sceneLoader.Save(scene, "quicksave.geisha-scene");
                    if (inputComponent.HardwareInput.KeyboardInput.F9) _sceneManager.LoadScene("quicksave.geisha-scene");

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
        }

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
        }

        public void OnComponentRemoved(Component component)
        {
        }
    }
}