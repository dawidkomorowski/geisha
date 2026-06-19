namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class KinematicIntegration
{
    public static void IntegrateKinematicMotion(ref PhysicsSceneData scene, double deltaTimeSeconds)
    {
        foreach (var index in scene.KinematicBodyIndices)
        {
            ref var body = ref scene.BodiesSpan[index];
            body.Position += body.LinearVelocity * deltaTimeSeconds;
            body.Rotation += body.AngularVelocity * deltaTimeSeconds;
        }
    }
}