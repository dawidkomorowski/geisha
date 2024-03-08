using System.Collections.Generic;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class CollisionDetection
{
    public static void DetectCollisions(IReadOnlyList<RigidBody2D> staticBodies, IReadOnlyList<RigidBody2D> kinematicBodies)
    {
        for (var i = 0; i < staticBodies.Count; i++)
        {
            var staticBody = staticBodies[i];
            staticBody.Contacts.Clear();
        }

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];
            kinematicBody.Contacts.Clear();
        }

        DetectCollisions_Kinematic_Vs_Kinematic(kinematicBodies);
        DetectCollisions_Kinematic_Vs_Static(kinematicBodies, staticBodies);
    }

    private static void DetectCollisions_Kinematic_Vs_Kinematic(IReadOnlyList<RigidBody2D> kinematicBodies)
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
                    overlaps = kinematicBody1.TransformedCircleCollider.Overlaps(kinematicBody2.TransformedCircleCollider);
                }
                else if (kinematicBody1.IsRectangleCollider && kinematicBody2.IsRectangleCollider)
                {
                    overlaps = kinematicBody1.TransformedRectangleCollider.Overlaps(kinematicBody2.TransformedRectangleCollider);
                }
                else if (kinematicBody1.IsCircleCollider && kinematicBody2.IsRectangleCollider)
                {
                    overlaps = kinematicBody1.TransformedCircleCollider.Overlaps(kinematicBody2.TransformedRectangleCollider);
                }
                else if (kinematicBody1.IsRectangleCollider && kinematicBody2.IsCircleCollider)
                {
                    overlaps = kinematicBody1.TransformedRectangleCollider.Overlaps(kinematicBody2.TransformedCircleCollider);
                }

                if (overlaps)
                {
                    // TODO
                    //kinematicBody1.Collider.AddCollidingEntity(kinematicBody2.Entity);
                    //kinematicBody2.Collider.AddCollidingEntity(kinematicBody1.Entity);
                }
            }
        }
    }

    private static void DetectCollisions_Kinematic_Vs_Static(IReadOnlyList<RigidBody2D> kinematicBodies, IReadOnlyList<RigidBody2D> staticBodies)
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
                    overlaps = kinematicBody.TransformedCircleCollider.Overlaps(staticBody.TransformedCircleCollider, out var separationInfo);

                    if (overlaps)
                    {
                        var contactPoint = ContactGenerator.GenerateContactForCircleVsCircle(
                            kinematicBody.TransformedCircleCollider,
                            staticBody.TransformedCircleCollider,
                            separationInfo
                        );
                        var contact = new Contact(kinematicBody, staticBody, contactPoint);
                        kinematicBody.Contacts.Add(contact);
                    }
                }
                else if (kinematicBody.IsRectangleCollider && staticBody.IsRectangleCollider)
                {
                    overlaps = kinematicBody.TransformedRectangleCollider.Overlaps(staticBody.TransformedRectangleCollider, out var separationInfo);

                    if (overlaps)
                    {
                        var contact = ContactGenerator.GenerateContact(kinematicBody, staticBody, separationInfo);
                        kinematicBody.Contacts.Add(contact);
                    }
                }
                else if (kinematicBody.IsCircleCollider && staticBody.IsRectangleCollider)
                {
                    overlaps = kinematicBody.TransformedCircleCollider.Overlaps(staticBody.TransformedRectangleCollider, out var separationInfo);

                    if (overlaps)
                    {
                        var contactPoint = ContactGenerator.GenerateContactForCircleVsRectangle(
                            kinematicBody.TransformedCircleCollider,
                            staticBody.TransformedRectangleCollider,
                            separationInfo
                        );
                        var contact = new Contact(kinematicBody, staticBody, contactPoint);
                        kinematicBody.Contacts.Add(contact);
                    }
                }
                else if (kinematicBody.IsRectangleCollider && staticBody.IsCircleCollider)
                {
                    overlaps = kinematicBody.TransformedRectangleCollider.Overlaps(staticBody.TransformedCircleCollider, out var separationInfo);

                    if (overlaps)
                    {
                        var contactPoint = ContactGenerator.GenerateContactForRectangleVsCircle(
                            kinematicBody.TransformedRectangleCollider,
                            staticBody.TransformedCircleCollider,
                            separationInfo
                        );
                        var contact = new Contact(kinematicBody, staticBody, contactPoint);
                        kinematicBody.Contacts.Add(contact);
                    }
                }

                if (overlaps)
                {
                    // TODO
                    //kinematicBody.Collider.AddCollidingEntity(staticBody.Entity);
                    //staticBody.Collider.AddCollidingEntity(kinematicBody.Entity);
                }
            }
        }
    }
}