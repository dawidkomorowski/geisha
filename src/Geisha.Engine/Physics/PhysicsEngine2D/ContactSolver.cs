using System.Collections.Generic;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class ContactSolver
{
    public static void SolvePositionConstraints(IReadOnlyList<RigidBody2D> kinematicBodies)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            if (kinematicBody.EnableCollisionResponse is false)
            {
                continue;
            }

            foreach (var contact in kinematicBody.Contacts)
            {
                var pc = PositionConstraint(contact);

                // TODO This threshold is arbitrary. It could be configurable.
                if (pc < 0)
                {
                    continue;
                }

                var translation = contact.CollisionNormal * pc;

                if (contact.Body2 == kinematicBody)
                {
                    translation = -translation;
                }

                kinematicBody.Position += translation;
            }

            kinematicBody.RecomputeCollider();
        }
    }

    private static double PositionConstraint(Contact contact)
    {
        var cp = contact.ContactPoints[0];
        var p1 = contact.Body1.Position + cp.LocalPosition1;
        var p2 = contact.Body2.Position + cp.LocalPosition2;
        var tv = p1 - p2;
        return contact.PenetrationDepth - tv.Dot(contact.CollisionNormal);
    }
}