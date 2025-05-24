using Geisha.Engine.Core.Math;
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

                var (overlap, mtv) = TestOverlap(kinematicBody1, kinematicBody2);

                if (overlap is false)
                {
                    continue;
                }

                var contact = ContactGenerator.GenerateContact(kinematicBody1, kinematicBody2, mtv);
                kinematicBody1.Contacts.Add(contact);
                kinematicBody2.Contacts.Add(contact);
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

                var (overlap, mtv) = TestOverlap(kinematicBody, staticBody);

                if (overlap is false)
                {
                    continue;
                }

                var contact = ContactGenerator.GenerateContact(kinematicBody, staticBody, mtv);
                kinematicBody.Contacts.Add(contact);
                staticBody.Contacts.Add(contact);
            }
        }
    }

    private static (bool overlap, MinimumTranslationVector mtv) TestOverlap(RigidBody2D body1, RigidBody2D body2)
    {
        if (!body1.BoundingRectangle.Overlaps(body2.BoundingRectangle))
        {
            return (false, default);
        }

        var overlap = false;
        var mtv = new MinimumTranslationVector();

        if (body1.IsCircleCollider && body2.IsCircleCollider)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedCircleCollider, out mtv);
        }
        else if (body1.IsRectangleCollider && body2.IsRectangleCollider)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedRectangleCollider, out mtv);
        }
        else if (body1.IsCircleCollider && body2.IsRectangleCollider)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedRectangleCollider, out mtv);
        }
        else if (body1.IsRectangleCollider && body2.IsCircleCollider)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedCircleCollider, out mtv);
        }

        return (overlap, mtv);
    }
}