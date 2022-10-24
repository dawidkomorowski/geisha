using System.Collections.Generic;

namespace Geisha.Engine.Physics.Systems
{
    internal sealed class CollisionDetection
    {
        public void DetectCollision(IReadOnlyList<PhysicsBody> physicsBodies)
        {
            foreach (var physicsBody in physicsBodies)
            {
                physicsBody.Collider.ClearCollidingEntities();
            }

            for (var i = 0; i < physicsBodies.Count; i++)
            {
                var physicsBody1 = physicsBodies[i];

                for (var j = i + 1; j < physicsBodies.Count; j++)
                {
                    var physicsBody2 = physicsBodies[j];

                    var overlaps = false;
                    if (physicsBody1.IsCircleCollider && physicsBody2.IsCircleCollider)
                    {
                        overlaps = physicsBody1.TransformedCircle.FastOverlaps(physicsBody2.TransformedCircle);
                    }
                    else if (physicsBody1.IsRectangleCollider && physicsBody2.IsRectangleCollider)
                    {
                        overlaps = physicsBody1.TransformedRectangle.FastOverlaps(physicsBody2.TransformedRectangle);
                    }
                    else if (physicsBody1.IsCircleCollider && physicsBody2.IsRectangleCollider)
                    {
                        overlaps = physicsBody1.TransformedCircle.FastOverlaps(physicsBody2.TransformedRectangle);
                    }
                    else if (physicsBody1.IsRectangleCollider && physicsBody2.IsCircleCollider)
                    {
                        overlaps = physicsBody1.TransformedRectangle.FastOverlaps(physicsBody2.TransformedCircle);
                    }

                    if (overlaps)
                    {
                        physicsBody1.Collider.AddCollidingEntity(physicsBody2.Entity);
                        physicsBody2.Collider.AddCollidingEntity(physicsBody1.Entity);
                    }
                }
            }
        }
    }
}