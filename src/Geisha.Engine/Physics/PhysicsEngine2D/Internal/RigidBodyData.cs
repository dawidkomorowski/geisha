using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct RigidBodyData : IUnmanaged<RigidBodyData>
{
    public BodyType Type;

    public Vector2 Position;
    public double Rotation;
    public Vector2 LinearVelocity;
    public double AngularVelocity;

    public bool EnableCollisionDetection;
    public bool EnableCollisionResponse;

    public bool IsSensor;

    public uint CollisionLayer;
    public uint CollisionMask;
}