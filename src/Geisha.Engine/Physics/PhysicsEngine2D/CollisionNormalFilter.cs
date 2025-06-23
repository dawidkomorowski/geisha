using System;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

[Flags]
internal enum CollisionNormalFilter
{
    None = 0,
    NegativeHorizontal = 1,
    PositiveHorizontal = 2,
    NegativeVertical = 4,
    PositiveVertical = 8,
    All = NegativeHorizontal | PositiveHorizontal | NegativeVertical | PositiveVertical
}