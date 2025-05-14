using System.Collections.Generic;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class KinematicIntegration
{
    public static void IntegrateKinematicMotion(IReadOnlyList<RigidBody2D> bodies, double deltaTimeSeconds)
    {
        for (var i = 0; i < bodies.Count; i++)
        {
            var body = bodies[i];

            body.Position += body.LinearVelocity * deltaTimeSeconds;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }
}