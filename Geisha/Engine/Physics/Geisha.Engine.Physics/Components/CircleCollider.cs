using Geisha.Common.Math;

namespace Geisha.Engine.Physics.Components
{
    // TODO add documentation
    // TODO Do not use circle as only radius needed
    public class CircleCollider : Collider2D
    {
        public Circle Circle { get; set; }
    }
}