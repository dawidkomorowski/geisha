namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class KinematicIntegration
{
    public static void IntegrateKinematicMotion(ref PhysicsSceneData scene, double deltaTimeSeconds)
    {
        foreach (ref var body in scene.KinematicBodiesSpan)
        {
            body.Position += body.LinearVelocity * deltaTimeSeconds;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }
}