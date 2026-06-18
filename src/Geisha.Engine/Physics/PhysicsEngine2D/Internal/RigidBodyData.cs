using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct RigidBodyData : IUnmanaged<RigidBodyData>
{
    public Vector2 Position;
    public double Rotation;

    public bool EnableCollisionDetection;
}