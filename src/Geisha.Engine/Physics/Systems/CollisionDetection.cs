using System.Collections.Generic;

namespace Geisha.Engine.Physics.Systems
{
    internal sealed class CollisionDetection
    {
        public void DetectCollisions(IReadOnlyList<KinematicBody> kinematicBodies)
        {
            foreach (var kinematicBody in kinematicBodies)
            {
                kinematicBody.Collider.ClearCollidingEntities();
            }

            for (var i = 0; i < kinematicBodies.Count; i++)
            {
                var kinematicBody1 = kinematicBodies[i];

                for (var j = i + 1; j < kinematicBodies.Count; j++)
                {
                    var kinematicBody2 = kinematicBodies[j];

                    var overlaps = false;
                    if (kinematicBody1.IsCircleCollider && kinematicBody2.IsCircleCollider)
                    {
                        overlaps = kinematicBody1.TransformedCircle.Overlaps(kinematicBody2.TransformedCircle);
                    }
                    else if (kinematicBody1.IsRectangleCollider && kinematicBody2.IsRectangleCollider)
                    {
                        overlaps = kinematicBody1.TransformedRectangle.Overlaps(kinematicBody2.TransformedRectangle);
                    }
                    else if (kinematicBody1.IsCircleCollider && kinematicBody2.IsRectangleCollider)
                    {
                        overlaps = kinematicBody1.TransformedCircle.Overlaps(kinematicBody2.TransformedRectangle);
                    }
                    else if (kinematicBody1.IsRectangleCollider && kinematicBody2.IsCircleCollider)
                    {
                        overlaps = kinematicBody1.TransformedRectangle.Overlaps(kinematicBody2.TransformedCircle);
                    }

                    if (overlaps)
                    {
                        kinematicBody1.Collider.AddCollidingEntity(kinematicBody2.Entity);
                        kinematicBody2.Collider.AddCollidingEntity(kinematicBody1.Entity);
                    }
                }
            }
        }
    }
}