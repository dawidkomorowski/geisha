﻿using System.Collections.Generic;

namespace Geisha.Engine.Physics.Systems;

internal static class CollisionDetection
{
    public static void DetectCollisions(PhysicsState physicsState)
    {
        var staticBodies = physicsState.GetStaticBodies();
        var kinematicBodies = physicsState.GetKinematicBodies();

        for (var i = 0; i < staticBodies.Count; i++)
        {
            var staticBody = staticBodies[i];
            staticBody.Collider.ClearCollidingEntities();
        }

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];
            kinematicBody.Collider.ClearCollidingEntities();
        }

        DetectCollisions_Kinematic_Vs_Kinematic(kinematicBodies);
        DetectCollisions_Kinematic_Vs_Static(kinematicBodies, staticBodies);
    }

    private static void DetectCollisions_Kinematic_Vs_Kinematic(IReadOnlyList<KinematicBody> kinematicBodies)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody1 = kinematicBodies[i];

            for (var j = i + 1; j < kinematicBodies.Count; j++)
            {
                var kinematicBody2 = kinematicBodies[j];

                if (!kinematicBody1.BoundingRectangle.Overlaps(kinematicBody2.BoundingRectangle))
                {
                    continue;
                }

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

    private static void DetectCollisions_Kinematic_Vs_Static(IReadOnlyList<KinematicBody> kinematicBodies, IReadOnlyList<StaticBody> staticBodies)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            for (var j = 0; j < staticBodies.Count; j++)
            {
                var staticBody = staticBodies[j];

                if (!kinematicBody.BoundingRectangle.Overlaps(staticBody.BoundingRectangle))
                {
                    continue;
                }

                var overlaps = false;
                if (kinematicBody.IsCircleCollider && staticBody.IsCircleCollider)
                {
                    overlaps = kinematicBody.TransformedCircle.Overlaps(staticBody.TransformedCircle);
                }
                else if (kinematicBody.IsRectangleCollider && staticBody.IsRectangleCollider)
                {
                    overlaps = kinematicBody.TransformedRectangle.Overlaps(staticBody.TransformedRectangle);
                }
                else if (kinematicBody.IsCircleCollider && staticBody.IsRectangleCollider)
                {
                    overlaps = kinematicBody.TransformedCircle.Overlaps(staticBody.TransformedRectangle);
                }
                else if (kinematicBody.IsRectangleCollider && staticBody.IsCircleCollider)
                {
                    overlaps = kinematicBody.TransformedRectangle.Overlaps(staticBody.TransformedCircle);
                }

                if (overlaps)
                {
                    kinematicBody.Collider.AddCollidingEntity(staticBody.Entity);
                    staticBody.Collider.AddCollidingEntity(kinematicBody.Entity);
                }
            }
        }
    }
}