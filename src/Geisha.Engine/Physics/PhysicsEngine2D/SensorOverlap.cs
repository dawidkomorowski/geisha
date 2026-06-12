namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal readonly struct SensorOverlap
{
    public SensorOverlap(RigidBody2D body1, RigidBody2D body2)
    {
        Body1 = body1;
        Body2 = body2;
    }

    public RigidBody2D Body1 { get; }
    public RigidBody2D Body2 { get; }
}