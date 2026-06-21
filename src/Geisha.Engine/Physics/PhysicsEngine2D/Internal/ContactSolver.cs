namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class ContactSolver
{
    public static void SolveVelocityConstraints(ref PhysicsSceneData scene)
    {
        foreach (ref var contact in scene.ContactsSpan)
        {
            ref var body1 = ref scene.GetBodyData(contact.Link1.BodyId);
            ref var body2 = ref scene.GetBodyData(contact.Link2.BodyId);

            var solveBody1 = body1.Type is BodyType.Kinematic && body1.EnableCollisionResponse;
            var solveBody2 = body2.Type is BodyType.Kinematic && body2.EnableCollisionResponse;

            if (!solveBody1 && !solveBody2)
            {
                continue;
            }

            var vc = VelocityConstraint(in contact, in body1, in body2);

            // TODO This threshold is arbitrary. It could be configurable.
            if (vc < 0)
            {
                continue;
            }

            var normalImpulse = contact.ContactManifold.CollisionNormal * vc;

            if (solveBody1 && solveBody2)
            {
                normalImpulse *= 0.5;
                body1.LinearVelocity += normalImpulse;
                body2.LinearVelocity -= normalImpulse;
            }
            else
            {
                if (solveBody1)
                {
                    body1.LinearVelocity += normalImpulse;
                }
                else
                {
                    body2.LinearVelocity -= normalImpulse;
                }
            }
        }
    }

    private static double VelocityConstraint(in ContactData contact, in RigidBodyData body1, in RigidBodyData body2)
    {
        var relativeVelocity = body1.LinearVelocity - body2.LinearVelocity;
        return -relativeVelocity.Dot(contact.ContactManifold.CollisionNormal);
    }

    public static void SolvePositionConstraints(ref PhysicsSceneData scene)
    {
        foreach (ref var contact in scene.ContactsSpan)
        {
            ref var body1 = ref scene.GetBodyData(contact.Link1.BodyId);
            ref var body2 = ref scene.GetBodyData(contact.Link2.BodyId);

            var solveBody1 = body1.Type is BodyType.Kinematic && body1.EnableCollisionResponse;
            var solveBody2 = body2.Type is BodyType.Kinematic && body2.EnableCollisionResponse;

            if (!solveBody1 && !solveBody2)
            {
                continue;
            }

            var pc = PositionConstraint(in contact, in body1, in body2) - scene.SimulationParameters.PenetrationTolerance;

            if (pc < 0)
            {
                continue;
            }

            var translation = contact.ContactManifold.CollisionNormal * pc;

            if (solveBody1 && solveBody2)
            {
                translation *= 0.5;
                body1.Position += translation;
                body2.Position -= translation;
            }
            else
            {
                if (solveBody1)
                {
                    body1.Position += translation;
                }
                else
                {
                    body2.Position -= translation;
                }
            }
        }
    }

    private static double PositionConstraint(in ContactData contact, in RigidBodyData body1, in RigidBodyData body2)
    {
        var cp = contact.ContactManifold.ContactPoints[0];
        var p1 = body1.Position + cp.LocalPosition1;
        var p2 = body2.Position + cp.LocalPosition2;
        var relativePosition = p1 - p2;
        return contact.ContactManifold.PenetrationDepth - relativePosition.Dot(contact.ContactManifold.CollisionNormal);
    }
}