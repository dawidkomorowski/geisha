using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class TopDownCameraForBoxComponent : BehaviorComponent
    {
        private Entity _box = null!;

        public TopDownCameraForBoxComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
            _box = Scene.AllEntities.Single(e => e.HasComponent<BoxMovementComponent>());
            SetCameraTransformAsBoxTransform();
        }

        public override void OnFixedUpdate()
        {
            SetCameraTransformAsBoxTransform();
        }

        private void SetCameraTransformAsBoxTransform()
        {
            var transform = Entity.GetComponent<Transform2DComponent>();
            var boxTransform = _box.GetComponent<Transform2DComponent>();

            transform.Translation = boxTransform.Translation;
            transform.Rotation = boxTransform.Rotation;
        }
    }

    internal sealed class TopDownCameraForBoxComponentFactory : ComponentFactory<TopDownCameraForBoxComponent>
    {
        protected override TopDownCameraForBoxComponent CreateComponent(Entity entity) => new(entity);
    }
}