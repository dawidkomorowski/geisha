using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    // TODO Introduce Transform2D and Transform3D?
    /// <summary>
    ///     Transform component represents set of geometrical transformations that include translation, rotation and scale.
    /// </summary>
    /// <remarks>Transform component allows to position objects in space by applying translation, rotation and scale to them.</remarks>
    public sealed class Transform : IComponent
    {
        /// <summary>
        ///     Translation along X, Y and Z axis from the origin of the coordinate system.
        /// </summary>
        public Vector3 Translation { get; set; }

        /// <summary>
        ///     Rotation around X, Y and Z axis of the coordinate system.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        ///     Scale along X, Y and Z axis of the coordinate system.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        ///     Unit vector pointing along X axis in local coordinate space.
        /// </summary>
        public Vector3 VectorX => (Matrix4.RotationZXY(Rotation) * Vector3.VectorX.Homogeneous).ToVector3();

        /// <summary>
        ///     Unit vector pointing along Y axis in local coordinate space.
        /// </summary>
        public Vector3 VectorY => (Matrix4.RotationZXY(Rotation) * Vector3.VectorY.Homogeneous).ToVector3();

        /// <summary>
        ///     Unit vector pointing along Z axis in local coordinate space.
        /// </summary>
        public Vector3 VectorZ => (Matrix4.RotationZXY(Rotation) * Vector3.VectorZ.Homogeneous).ToVector3();

        /// <summary>
        ///     Returns default transform that is zero translation, zero rotation and scale factor equal one.
        /// </summary>
        public static Transform Default => new Transform {Translation = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One};

        /// <summary>
        ///     Creates 2D transformation matrix that represents 2D part of this transform component.
        /// </summary>
        /// <returns>2D transformation matrix representing this transform.</returns>
        public Matrix3 Create2DTransformationMatrix()
        {
            return Matrix3.Translation(Translation.ToVector2()) * Matrix3.Rotation(Rotation.Z) * Matrix3.Scale(Scale.ToVector2()) * Matrix3.Identity;
        }

        /// <summary>
        ///     Creates 3D transformation matrix that represents this transform component.
        /// </summary>
        /// <returns>3D transformation matrix representing this transform.</returns>
        public Matrix4 Create3DTransformationMatrix()
        {
            return Matrix4.Translation(Translation) * Matrix4.RotationZXY(Rotation) * Matrix4.Scale(Scale) * Matrix4.Identity;
        }
    }
}