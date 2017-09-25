using Geisha.Common.Math;

namespace Geisha.Engine.Core.Components
{
    public class Transform : IComponent
    {
        public Vector3 Translation { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public Vector3 VectorX => (Matrix4.RotationZXY(Rotation) * Vector3.VectorX.Homogeneous).ToVector3();
        public Vector3 VectorY => (Matrix4.RotationZXY(Rotation) * Vector3.VectorY.Homogeneous).ToVector3();
        public Vector3 VectorZ => (Matrix4.RotationZXY(Rotation) * Vector3.VectorZ.Homogeneous).ToVector3();

        public static Transform Default => new Transform {Translation = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One};

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