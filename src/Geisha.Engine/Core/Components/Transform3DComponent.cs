using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using System;

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
    [ComponentId("Geisha.Engine.Core.Transform3DComponent")]
    public sealed class Transform3DComponent : Component
    {
        internal Transform3DComponent(Entity entity) : base(entity)
        {
            if (entity.HasComponent<Transform2DComponent>())
            {
                throw new ArgumentException($"{nameof(Transform2DComponent)} is already added to entity.");
            }

            if (entity.HasComponent<Transform3DComponent>())
            {
                throw new ArgumentException($"{nameof(Transform3DComponent)} is already added to entity.");
            }
        }

        /// <summary>
        ///     Translation along X, Y and Z axes from the origin of the local coordinate system. For root entities their local
        ///     coordinate system is the global coordinate system. Default value is zero.
        /// </summary>
        public Vector3 Translation { get; set; } = Vector3.Zero;

        /// <summary>
        ///     Counterclockwise rotation in radians around X, Y and Z axes of the local coordinate system. For root entities their
        ///     local coordinate system is the global coordinate system. Default value is zero.
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        /// <summary>
        ///     Scale along X, Y and Z axes of the local coordinate system. For root entities their local coordinate system is the
        ///     global coordinate system. Default value is one.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.One;

        // TODO Should it return vector in global space taking into account transform hierarchy?
        /// <summary>
        ///     Unit vector in parent's local coordinate system (or global coordinate system if there is no parent) pointing along
        ///     X axis of coordinate system defined by this <see cref="Transform3DComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
        ///     facing towards X axis in its own local coordinate system then after application of rotation this property gets
        ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
        ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
        ///     <see cref="VectorX" />.
        /// </remarks>
        public Vector3 VectorX => (Matrix4x4.CreateRotationZXY(Rotation) * Vector3.UnitX.Homogeneous).ToVector3();

        // TODO Should it return vector in global space taking into account transform hierarchy?
        /// <summary>
        ///     Unit vector in parent's local coordinate system (or global coordinate system if there is no parent) pointing along
        ///     Y axis of coordinate system defined by this <see cref="Transform3DComponent" />.
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
        ///     Unit vector in parent's local coordinate system (or global coordinate system if there is no parent) pointing along
        ///     Z axis of coordinate system defined by this <see cref="Transform3DComponent" />.
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
        ///     Creates 3D transformation matrix that represents this transform component.
        /// </summary>
        /// <returns>3D transformation matrix representing this transform component.</returns>
        public Matrix4x4 ToMatrix() =>
            Matrix4x4.CreateTranslation(Translation)
            * Matrix4x4.CreateRotationZXY(Rotation)
            * Matrix4x4.CreateScale(Scale)
            * Matrix4x4.Identity;

        /// <summary>
        ///     Serializes data of this instance of <see cref="Transform3DComponent" />.
        /// </summary>
        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteVector3("Translation", Translation);
            writer.WriteVector3("Rotation", Rotation);
            writer.WriteVector3("Scale", Scale);
        }

        /// <summary>
        ///     Deserializes data of this instance of <see cref="Transform3DComponent" />.
        /// </summary>
        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Translation = reader.ReadVector3("Translation");
            Rotation = reader.ReadVector3("Rotation");
            Scale = reader.ReadVector3("Scale");
        }
    }

    internal sealed class Transform3DComponentFactory : ComponentFactory<Transform3DComponent>
    {
        protected override Transform3DComponent CreateComponent(Entity entity) => new(entity);
    }
}