using System.Collections.Generic;
using System.Diagnostics;
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
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class SandboxSystem : ICustomSystem
    {
        private readonly IAssetStore _assetStore;
        private readonly IEngineManager _engineManager;
        private readonly ISceneLoader _sceneLoader;
        private readonly ISceneManager _sceneManager;
        private readonly IAudioPlayer _audioPlayer;

        private readonly List<Entity> _dieFromBoxEntities = new();

        private Entity? _box;
        private Entity? _mousePointer;
        private CameraComponent? _cameraComponent;
        private Entity? _closeGameOnEscapeKeyEntity;

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

        public void ProcessFixedUpdate()
        {
            Debug.Assert(_box != null, "_box != null");
            Debug.Assert(_mousePointer != null, "_mousePointer != null");
            Debug.Assert(_cameraComponent != null, "_cameraComponent != null");
            Debug.Assert(_closeGameOnEscapeKeyEntity != null, "_closeGameOnEscapeKeyEntity != null");

            foreach (var entity in _dieFromBoxEntities)
            {
                var collider = entity.GetComponent<CircleColliderComponent>();
                if (collider.IsColliding)
                {
                    var collidedWithBox = collider.CollidingEntities.Contains(_box);
                    var collidedWithMousePointer = collider.CollidingEntities.Contains(_mousePointer);
                    var mousePointerHasLeftButtonPressed = _mousePointer.GetComponent<MousePointerComponent>().LeftButtonPressed;

                    if (collidedWithBox || (collidedWithMousePointer && mousePointerHasLeftButtonPressed))
                    {
                        _audioPlayer.PlayOnce(_assetStore.GetAsset<ISound>(AssetsIds.SfxSound));
                        entity.RemoveAfterFixedTimeStep();
                    }
                }
            }


            var inputComponent = _closeGameOnEscapeKeyEntity.GetComponent<InputComponent>();
            if (inputComponent.HardwareInput.KeyboardInput.Escape) _engineManager.ScheduleEngineShutdown();
            if (inputComponent.HardwareInput.KeyboardInput.F5) _sceneLoader.Save(_sceneManager.CurrentScene, "quicksave.geisha-scene");
            if (inputComponent.HardwareInput.KeyboardInput.F9) _sceneManager.LoadScene("quicksave.geisha-scene");

            var mouseScrollDelta = inputComponent.HardwareInput.MouseInput.ScrollDelta;
            var scalingFactor = mouseScrollDelta == 0 ? 1 : mouseScrollDelta > 0 ? 10d / 11d : 11d / 10d;
            _cameraComponent.ViewRectangle *= scalingFactor;
        }

        public void ProcessUpdate(GameTime gameTime)
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
            switch (component)
            {
                case BoxMovementComponent _:
                    _box = component.Entity;
                    break;
                case MousePointerComponent _:
                    _mousePointer = component.Entity;
                    break;
                case CameraComponent cameraComponent:
                    _cameraComponent = cameraComponent;
                    break;
                case DieFromBoxComponent _:
                    _dieFromBoxEntities.Add(component.Entity);
                    break;
                case CloseGameOnEscapeKeyComponent _:
                    _closeGameOnEscapeKeyEntity = component.Entity;
                    break;
            }
        }

        public void OnComponentRemoved(Component component)
        {
            switch (component)
            {
                case DieFromBoxComponent _:
                    _dieFromBoxEntities.Remove(component.Entity);
                    break;
            }
        }
    }
}