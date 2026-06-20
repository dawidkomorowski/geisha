using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal struct ContactData : IUnmanaged<ContactData>
{
    public Link Link1;
    public Link Link2;
    public ContactManifold ContactManifold;

    public struct Link
    {
        public const int NullIndex = -1;

        public RigidBodyId BodyId;
        public int PrevIndex;
        public int NextIndex;
    }
}