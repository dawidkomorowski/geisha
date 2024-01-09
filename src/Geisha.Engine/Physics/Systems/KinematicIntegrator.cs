namespace Geisha.Engine.Physics.Systems;

internal static class KinematicIntegrator
{
    public static void IntegrateKinematicMotion(PhysicsState physicsState, double deltaTimeSeconds)
    {
        var kinematicBodies = physicsState.GetKinematicBodies();

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            kinematicBody.Position += kinematicBody.LinearVelocity * deltaTimeSeconds;
            kinematicBody.Rotation += kinematicBody.AngularVelocity * deltaTimeSeconds;
        }
    }
}