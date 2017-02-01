using Geisha.Common.Geometry;

namespace Geisha.Engine.Core.Components
{
    public class Transform : IComponent
    {
        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }
}