using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics;

// TODO Add documentation.
public class Contact2D
{
    public Collider2DComponent ThisCollider { get; }
    public Collider2DComponent OtherCollider { get; }
    public Vector2 CollisionNormal { get; } // TODO Is it from This to Other or from Other to This?
    public double SeparationDepth { get; } // TODO SeparationDepth or Separation or something else?
    public ReadOnlyFixedList2<ContactPoint2D> ContactPoints { get; }
}