using System.Collections.Generic;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class ContactSolver
{
    public static void SolveVelocityConstraints(IReadOnlyList<RigidBody2D> kinematicBodies)
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
                var vc = VelocityConstraint(contact);

                // TODO This threshold is arbitrary. It could be configurable.
                if (vc < 0)
                {
                    continue;
                }

                var normalImpulse = contact.CollisionNormal * vc;

                if (contact.Body1.Type is BodyType.Kinematic && contact.Body1.EnableCollisionResponse &&
                    contact.Body2.Type is BodyType.Kinematic && contact.Body2.EnableCollisionResponse)
                {
                    // TODO As both kinematic bodies share the same contact it is solved twice. This is not optimal.
                    normalImpulse *= 0.5;
                    contact.Body1.LinearVelocity += normalImpulse;
                    contact.Body2.LinearVelocity -= normalImpulse;
                }
                else
                {
                    if (contact.Body2 == kinematicBody)
                    {
                        normalImpulse = -normalImpulse;
                    }

                    kinematicBody.LinearVelocity += normalImpulse;
                }
            }
        }
    }

    private static double VelocityConstraint(Contact contact)
    {
        var relativeVelocity = contact.Body1.LinearVelocity - contact.Body2.LinearVelocity;
        return -relativeVelocity.Dot(contact.CollisionNormal);
    }

    public static void SolvePositionConstraints(IReadOnlyList<RigidBody2D> kinematicBodies, double penetrationTolerance)
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
                var pc = PositionConstraint(contact) - penetrationTolerance;

                if (pc < 0)
                {
                    continue;
                }

                var translation = contact.CollisionNormal * pc;

                if (contact.Body1.Type is BodyType.Kinematic && contact.Body1.EnableCollisionResponse &&
                    contact.Body2.Type is BodyType.Kinematic && contact.Body2.EnableCollisionResponse)
                {
                    // TODO As both kinematic bodies share the same contact it is solved twice. This is not optimal.
                    translation *= 0.5;
                    contact.Body1.Position += translation;
                    contact.Body2.Position -= translation;
                }
                else
                {
                    if (contact.Body2 == kinematicBody)
                    {
                        translation = -translation;
                    }

                    kinematicBody.Position += translation;
                }
            }

            kinematicBody.RecomputeCollider();
        }
    }

    private static double PositionConstraint(Contact contact)
    {
        var cp = contact.ContactPoints[0];
        var p1 = contact.Body1.Position + cp.LocalPosition1;
        var p2 = contact.Body2.Position + cp.LocalPosition2;
        var relativePosition = p1 - p2;
        return contact.PenetrationDepth - relativePosition.Dot(contact.CollisionNormal);
    }
}