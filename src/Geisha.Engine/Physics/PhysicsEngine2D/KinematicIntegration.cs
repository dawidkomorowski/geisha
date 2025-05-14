using System.Collections.Generic;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class KinematicIntegration
{
    public static void IntegrateKinematicMotion(IReadOnlyList<RigidBody2D> kinematicBodies, double deltaTimeSeconds)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var body = kinematicBodies[i];

            body.Position += body.LinearVelocity * deltaTimeSeconds;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }
}