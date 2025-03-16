using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class SandboxSystem : ICustomSystem
    {
        private CameraComponent? _cameraComponent;
        private InputComponent? _inputComponent;

        public string Name => nameof(SandboxSystem);

        public void ProcessFixedUpdate()
        {
            Debug.Assert(_cameraComponent != null, "_cameraComponent != null");
            Debug.Assert(_inputComponent != null, nameof(_inputComponent) + " != null");

            var mouseScrollDelta = _inputComponent.HardwareInput.MouseInput.ScrollDelta;
            var scalingFactor = mouseScrollDelta == 0 ? 1 : mouseScrollDelta > 0 ? 10d / 11d : 11d / 10d;
            _cameraComponent.ViewRectangle *= scalingFactor;

            //var transform = _cameraComponent.Entity.Scene.AllEntities.Single(e => e.HasComponent<EllipseRendererComponent>())
            //    .GetComponent<Transform2DComponent>();
            //transform.Translation = _cameraComponent.ScreenPointToWorld2DPoint(_inputComponent.HardwareInput.MouseInput.Position);
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
                case CameraComponent cameraComponent:
                    _cameraComponent = cameraComponent;
                    break;
                case InputComponent inputComponent:
                    _inputComponent = inputComponent;
                    break;
            }
        }

        public void OnComponentRemoved(Component component)
        {
        }
    }
}