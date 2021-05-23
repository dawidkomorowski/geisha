using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components
{
    /// <summary>
    ///     <see cref="Transform2DComponent" /> represents set of 2D geometrical transformations that include translation,
    ///     rotation and scale.
    /// </summary>
    /// <remarks>
    ///     <see cref="Transform2DComponent" /> allows to position objects in space by applying translation, rotation and
    ///     scale to them. <see cref="Transform2DComponent" /> applies transformation that moves the object relative to the
    ///     origin of the coordinate system. For root entities it is global coordinate system so transform is relative to the
    ///     scene. For child entities (having a parent with <see cref="Transform2DComponent" />) it is parent's local
    ///     coordinate system so transform is relative to parent entity. <see cref="Transform2DComponent" /> effectively
    ///     defines new local coordinate system for child entities.
    /// </remarks>
    [ComponentId("Geisha.Engine.Core.Transform2DComponent")]
    public sealed class Transform2DComponent : Component
    {
        /// <summary>
        ///     Translation along X and Y axes from the origin of the local coordinate system. For root entities their local
        ///     coordinate system is the global coordinate system.
        /// </summary>
        public Vector2 Translation { get; set; }

        /// <summary>
        ///     Rotation in radians around the origin of the local coordinate system. For root entities their local coordinate
        ///     system is the global coordinate system.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        ///     Scale along X and Y axes of the local coordinate system. For root entities their local coordinate system is the
        ///     global coordinate system.
        /// </summary>
        public Vector2 Scale { get; set; }

        /// <summary>
        ///     Unit vector in local coordinate system pointing along X axis of coordinate system defined by this
        ///     <see cref="Transform2DComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
        ///     facing towards X axis in its own local coordinate system then after application of rotation this property gets
        ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
        ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
        ///     <see cref="VectorX" />.
        /// </remarks>
        public Vector2 VectorX => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitX.Homogeneous).ToVector2();

        /// <summary>
        ///     Unit vector in local coordinate system pointing along Y axis of coordinate system defined by this
        ///     <see cref="Transform2DComponent" />.
        /// </summary>
        /// <remarks>
        ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
        ///     facing towards Y axis in its own local coordinate system then after application of rotation this property gets
        ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
        ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
        ///     <see cref="VectorX" />.
        /// </remarks>
        public Vector2 VectorY => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitY.Homogeneous).ToVector2();

        /// <summary>
        ///     Creates new instance of <see cref="Transform2DComponent" /> with default values that is zero translation, zero
        ///     rotation and scale factor equal one. It is identity transform.
        /// </summary>
        /// <returns><see cref="Transform2DComponent" /> instance with zero translation, zero rotation and scale factor equal one.</returns>
        public static Transform2DComponent CreateDefault() => new Transform2DComponent {Translation = Vector2.Zero, Rotation = 0, Scale = Vector2.One};

        /// <summary>
        ///     Creates 2D transformation matrix that represents this transform component.
        /// </summary>
        /// <returns>2D transformation matrix representing this transform component.</returns>
        public Matrix3x3 ToMatrix() =>
            Matrix3x3.CreateTranslation(Translation)
            * Matrix3x3.CreateRotation(Rotation)
            * Matrix3x3.CreateScale(Scale)
            * Matrix3x3.Identity;

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);
            componentDataWriter.WriteVector2("Translation", Translation);
            componentDataWriter.WriteDouble("Rotation", Rotation);
            componentDataWriter.WriteVector2("Scale", Scale);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);
            Translation = componentDataReader.ReadVector2("Translation");
            Rotation = componentDataReader.ReadDouble("Rotation");
            Scale = componentDataReader.ReadVector2("Scale");
        }
    }

    internal sealed class Transform2DComponentFactory : ComponentFactory<Transform2DComponent>
    {
        protected override Transform2DComponent CreateComponent() => new Transform2DComponent();
    }
}