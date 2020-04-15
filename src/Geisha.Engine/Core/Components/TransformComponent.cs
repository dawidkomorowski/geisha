using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Components
{
    // TODO Introduce Transform2DComponent and Transform3DComponent?
    /// <summary>
    ///     Transform component represents set of geometrical transformations that include translation, rotation and scale.
    /// </summary>
    /// <remarks>Transform component allows to position objects in space by applying translation, rotation and scale to them.</remarks>
    public sealed class TransformComponent : IComponent
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
        ///     Unit vector in global coordinate system pointing along X axis of local coordinate system.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If object is
        ///     facing towards X axis in local coordinate system then after application of rotation this property gets vector of
        ///     where the object is facing in global coordinate system. It can be used to easily move an object along the direction
        ///     it is facing by moving it along <see cref="VectorX" />.
        /// </remarks>
        public Vector3 VectorX => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitX.Homogeneous).ToVector3();

        /// <summary>
        ///     Unit vector in global coordinate system pointing along Y axis of local coordinate system.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If object is
        ///     facing towards Y axis in local coordinate system then after application of rotation this property gets vector of
        ///     where the object is facing in global coordinate system. It can be used to easily move an object along the direction
        ///     it is facing by moving it along <see cref="VectorY" />.
        /// </remarks>
        public Vector3 VectorY => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitY.Homogeneous).ToVector3();

        /// <summary>
        ///     Unit vector in global coordinate system pointing along Z axis of local coordinate system.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If object is
        ///     facing towards Z axis in local coordinate system then after application of rotation this property gets vector of
        ///     where the object is facing in global coordinate system. It can be used to easily move an object along the direction
        ///     it is facing by moving it along <see cref="VectorZ" />.
        /// </remarks>
        public Vector3 VectorZ => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitZ.Homogeneous).ToVector3();

        /// <summary>
        ///     Creates new instance of <see cref="TransformComponent"/> with default values that is zero translation, zero rotation and scale factor
        ///     equal one.
        /// </summary>
        /// <returns><see cref="TransformComponent" /> instance with zero translation, zero rotation and scale factor equal one.</returns>
        public static TransformComponent CreateDefault()
        {
            return new TransformComponent {Translation = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One};
        }

        /// <summary>
        ///     Creates 2D transformation matrix that represents 2D part of this transform component.
        /// </summary>
        /// <returns>2D transformation matrix representing this transform.</returns>
        public Matrix3x3 Create2DTransformationMatrix()
        {
            return Matrix3x3.CreateTranslation(Translation.ToVector2())
                   * Matrix3x3.CreateRotation(Rotation.Z)
                   * Matrix3x3.CreateScale(Scale.ToVector2())
                   * Matrix3x3.Identity;
        }

        /// <summary>
        ///     Creates 3D transformation matrix that represents this transform component.
        /// </summary>
        /// <returns>3D transformation matrix representing this transform.</returns>
        public Matrix4x4 Create3DTransformationMatrix()
        {
            return Matrix4x4.CreateTranslation(Translation)
                   * Matrix4x4.CreateRotationZXY(Rotation)
                   * Matrix4x4.CreateScale(Scale)
                   * Matrix4x4.Identity;
        }
    }
}