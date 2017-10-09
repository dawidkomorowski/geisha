using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.TestGame.Behaviors
{
    public class DieFromBox : Behavior
    {
        private Entity _box;
        private Scene _scene;

        public override void OnStart()
        {
            _scene = Entity.Scene;
            _box = _scene.AllEntities.Single(e => e.HasComponent<BoxMovement>());
        }

        public override void OnFixedUpdate()
        {
            var collider = Entity.GetComponent<CircleCollider>();

            if (collider.IsColliding && collider.CollidingEntities.Contains(_box))
                Entity.Destroy();
        }
    }
}