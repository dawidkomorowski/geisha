using Geisha.Common.Geometry;

namespace Geisha.Engine.Core.Components
{
    public class Transform : IComponent
    {
        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Matrix3 Create2DTransformationMatrix()
        {
            return Matrix3.Translation(Translation.ToVector2()) * Matrix3.Rotation(Rotation.Z) * Matrix3.Scale(Scale.ToVector2()) * Matrix3.Identity;
        }

        public Matrix4 Create3DTransformationMatrix()
        {
            return Matrix4.Translation(Translation) * Matrix4.RotationZXY(Rotation) * Matrix4.Scale(Scale) * Matrix4.Identity;
        }
    }
}