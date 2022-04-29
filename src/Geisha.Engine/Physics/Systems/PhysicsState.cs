using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    internal sealed class PhysicsState
    {
        private readonly List<PhysicsBody> _bodies = new List<PhysicsBody>();
        private readonly Dictionary<Entity, PhysicsBody> _index = new Dictionary<Entity, PhysicsBody>();
        private readonly Dictionary<Entity, PendingBody> _pendingBodies = new Dictionary<Entity, PendingBody>();

        public IReadOnlyList<PhysicsBody> GetPhysicsBodies() => _bodies;

        public void CreateStateFor(Transform2DComponent transform2DComponent)
        {
            var entity = transform2DComponent.Entity;

            if (_pendingBodies.TryGetValue(entity, out var pendingBody))
            {
                pendingBody.Transform = transform2DComponent;
            }
            else
            {
                pendingBody = new PendingBody
                {
                    Transform = transform2DComponent
                };
                _pendingBodies.Add(entity, pendingBody);
            }

            if (pendingBody.IsReady)
            {
                _pendingBodies.Remove(entity);

                Debug.Assert(pendingBody.Collider != null, "pendingBody.Collider != null");
                var physicsBody = new PhysicsBody(pendingBody.Transform, pendingBody.Collider);
                _bodies.Add(physicsBody);
                _index.Add(physicsBody.Entity, physicsBody);
            }
        }

        public void CreateStateFor(Collider2DComponent collider2DComponent)
        {
            var entity = collider2DComponent.Entity;

            if (_pendingBodies.TryGetValue(entity, out var pendingBody))
            {
                pendingBody.Collider = collider2DComponent;
            }
            else
            {
                pendingBody = new PendingBody
                {
                    Collider = collider2DComponent
                };
                _pendingBodies.Add(entity, pendingBody);
            }

            if (pendingBody.IsReady)
            {
                _pendingBodies.Remove(entity);

                Debug.Assert(pendingBody.Transform != null, "pendingBody.Transform != null");
                var physicsBody = new PhysicsBody(pendingBody.Transform, pendingBody.Collider);
                _bodies.Add(physicsBody);
                _index.Add(physicsBody.Entity, physicsBody);
            }
        }

        public void RemoveStateFor(Transform2DComponent transform2DComponent)
        {
        }

        public void RemoveStateFor(Collider2DComponent collider2DComponent)
        {
        }

        private sealed class PendingBody
        {
            public Transform2DComponent? Transform { get; set; }
            public Collider2DComponent? Collider { get; set; }

            public bool IsReady => Transform != null && Collider != null;
        }
    }
}