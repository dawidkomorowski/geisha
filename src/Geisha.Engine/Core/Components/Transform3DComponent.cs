using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components
{
    /// <summary>
    ///     <see cref="Transform3DComponent" /> represents set of 3D geometrical transformations that include translation,
    ///     rotation and scale.
    /// </summary>
    /// <remarks>
    ///     <see cref="Transform3DComponent" /> allows to position objects in space by applying translation, rotation and
    ///     scale to them. <see cref="Transform3DComponent" /> applies transformation that moves the object relative to the
    ///     origin of the coordinate system. For root entities it is global coordinate system so transform is relative to the
    ///     scene. For child entities (having a parent with <see cref="Transform3DComponent" />) it is parent's local
    ///     coordinate system so transform is relative to parent entity. <see cref="Transform3DComponent" /> effectively
    ///     defines new local coordinate system for child entities.
    /// </remarks>
    public sealed class Transform3DComponent : IComponent
    {
        public static ComponentId Id { get; } = new ComponentId("Geisha.Engine.Core.Transform3DComponent");

        public ComponentId ComponentId => Id;

        /// <summary>
        ///     Translation along X, Y and Z axes from the origin of the local coordinate system. For root entities their local
        ///     coordinate system is the global coordinate system.
        /// </summary>
        public Vector3 Translation { get; set; }

        /// <summary>
        ///     Rotation in radians around X, Y and Z axes of the local coordinate system. For root entities their local coordinate
        ///     system is the global coordinate system.
        /// </summary>
        public Vector3 Rotation { get; set; }

        /// <summary>
        ///     Scale along X, Y and Z axes of the local coordinate system. For root entities their local coordinate system is the
        ///     global coordinate system.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        ///     Unit vector in local coordinate system pointing along X axis of coordinate system defined by this
        ///     <see cref="Transform3DComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
        ///     facing towards X axis in its own local coordinate system then after application of rotation this property gets
        ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
        ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
        ///     <see cref="VectorX" />.
        /// </remarks>
        public Vector3 VectorX => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitX.Homogeneous).ToVector3();

        /// <summary>
        ///     Unit vector in local coordinate system pointing along Y axis of coordinate system defined by this
        ///     <see cref="Transform3DComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
        ///     facing towards Y axis in its own local coordinate system then after application of rotation this property gets
        ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
        ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
        ///     <see cref="VectorX" />.
        /// </remarks>
        public Vector3 VectorY => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitY.Homogeneous).ToVector3();

        /// <summary>
        ///     Unit vector in local coordinate system pointing along Z axis of coordinate system defined by this
        ///     <see cref="Transform3DComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
        ///     facing towards Z axis in its own local coordinate system then after application of rotation this property gets
        ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
        ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
        ///     <see cref="VectorX" />.
        /// </remarks>
        public Vector3 VectorZ => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitZ.Homogeneous).ToVector3();


        /// <summary>
        ///     Creates new instance of <see cref="Transform3DComponent" /> with default values that is zero translation, zero
        ///     rotation and scale factor equal one. It is identity transform.
        /// </summary>
        /// <returns><see cref="Transform3DComponent" /> instance with zero translation, zero rotation and scale factor equal one.</returns>
        public static Transform3DComponent CreateDefault() => new Transform3DComponent
            {Translation = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One};

        /// <summary>
        ///     Creates 3D transformation matrix that represents this transform component.
        /// </summary>
        /// <returns>3D transformation matrix representing this transform component.</returns>
        public Matrix4x4 ToMatrix() =>
            Matrix4x4.CreateTranslation(Translation)
            * Matrix4x4.CreateRotationZXY(Rotation)
            * Matrix4x4.CreateScale(Scale)
            * Matrix4x4.Identity;
    }

    internal sealed class Transform3DComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(Transform3DComponent);
        public ComponentId ComponentId => Transform3DComponent.Id;
        public IComponent Create() => new Transform3DComponent();
    }

    internal sealed class Transform3DComponentSerializer : ComponentSerializer<Transform3DComponent>
    {
        private const string Translation = "Translation";
        private const string Rotation = "Rotation";
        private const string Scale = "Scale";

        public Transform3DComponentSerializer() : base(Transform3DComponent.Id)
        {
        }

        protected override void Serialize(Transform3DComponent component, IComponentDataWriter componentDataWriter)
        {
            componentDataWriter.WriteVector3(Translation, component.Translation);
            componentDataWriter.WriteVector3(Rotation, component.Rotation);
            componentDataWriter.WriteVector3(Scale, component.Scale);
        }

        protected override void Deserialize(Transform3DComponent component, IComponentDataReader componentDataReader)
        {
            component.Translation = componentDataReader.ReadVector3(Translation);
            component.Rotation = componentDataReader.ReadVector3(Rotation);
            component.Scale = componentDataReader.ReadVector3(Scale);
        }
    }
}