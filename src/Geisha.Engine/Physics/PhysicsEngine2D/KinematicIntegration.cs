using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class KinematicIntegration
{
    public static void IntegrateKinematicMotion(ReadOnlySpan<RigidBody2D> kinematicBodies, double deltaTimeSeconds)
    {
        foreach (var body in kinematicBodies)
        {
            body.Position += body.LinearVelocity * deltaTimeSeconds;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }
}