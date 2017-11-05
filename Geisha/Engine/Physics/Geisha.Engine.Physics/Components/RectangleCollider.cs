using Geisha.Common.Math;

namespace Geisha.Engine.Physics.Components
{
    // TODO add documentation
    // TODO Do not use rectangle as only dimension needed
    public class RectangleCollider : Collider2D
    {
        public Rectangle Rectangle { get; set; }
    }
}