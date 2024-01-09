using Geisha.Engine.Core;

namespace Geisha.Engine.Physics.Systems;

internal static class KinematicIntegrator
{
    public static void IntegrateKinematicMotion(PhysicsState physicsState)
    {
        var kinematicBodies = physicsState.GetKinematicBodies();

        // TODO Should time step used by physics system be somehow centralized?
        var timeStep = GameTime.FixedDeltaTimeSeconds;

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            kinematicBody.Position += kinematicBody.LinearVelocity * timeStep;
            kinematicBody.Rotation += kinematicBody.AngularVelocity * timeStep;
        }
    }
}