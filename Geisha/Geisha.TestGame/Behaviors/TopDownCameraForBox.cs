using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.TestGame.Behaviors
{
    public class TopDownCameraForBox : Behavior
    {
        private Entity _box;

        public override void OnStart()
        {
            _box = Entity.Scene.AllEntities.Single(e => e.HasComponent<BoxMovement>());
        }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            var boxTransform = _box.GetComponent<Transform>();

            transform.Translation = boxTransform.Translation;
            transform.Rotation = boxTransform.Rotation;
        }
    }
}