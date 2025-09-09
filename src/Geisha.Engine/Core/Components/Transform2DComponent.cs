using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core.Components;

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
    // TODO This could be replaced with field keyword in .NET 10 (C# 14).
    private bool _isInterpolated;

    internal Transform2DComponent(Entity entity) : base(entity)
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

    internal TransformInterpolationSystem? TransformInterpolationSystem { get; set; }
    internal TransformInterpolationId TransformInterpolationId { get; set; } = TransformInterpolationId.Invalid;

    /// <summary>
    ///     Translation along X and Y axes from the origin of the local coordinate system. For root entities their local
    ///     coordinate system is the global coordinate system. Default value is zero.
    /// </summary>
    public Vector2 Translation { get; set; } = Vector2.Zero;

    /// <summary>
    ///     Counterclockwise rotation in radians around the origin of the local coordinate system. For root entities their
    ///     local coordinate system is the global coordinate system. Default value is zero.
    /// </summary>
    public double Rotation { get; set; }

    /// <summary>
    ///     Scale along X and Y axes of the local coordinate system. For root entities their local coordinate system is the
    ///     global coordinate system. Default value is one.
    /// </summary>
    public Vector2 Scale { get; set; } = Vector2.One;

    /// <summary>
    ///     Gets or sets <see cref="Transform" /> of this <see cref="Transform2DComponent" />.
    /// </summary>
    /// <remarks>
    ///     This property allows to manipulate <see cref="Translation" />, <see cref="Rotation" /> and <see cref="Scale" />
    ///     through compound representation in form of <see cref="Transform2D" />.
    /// </remarks>
    public Transform2D Transform
    {
        get => new(Translation, Rotation, Scale);
        set
        {
            Translation = value.Translation;
            Rotation = value.Rotation;
            Scale = value.Scale;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the transform is interpolated. Default value is <see langword="false" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Setting this property to <see langword="true" /> enables interpolation of the transform. Setting it to
    ///         <see langword="false" /> disables interpolation of the transform.
    ///     </para>
    ///     <para>
    ///         Use <see cref="InterpolatedTransform" /> to get current state of interpolated transform. Use
    ///         <see cref="Transform" /> to get or set exact transform state. Use
    ///         <see cref="SetTransformImmediate(Transform2D)" /> to set exact transform state and bypass interpolation.
    ///     </para>
    ///     <para>
    ///         When interpolation is enabled the transform is smoothly interpolated between its previous and current state by
    ///         the <see cref="Geisha.Engine.Core.Systems.TransformInterpolationSystem" />. The system takes snapshot of the
    ///         transform after each fixed update. During each frame update the system interpolates the transform based on the
    ///         time elapsed since the last fixed update (remaining time to simulate to get to the next fixed update).
    ///         Rendering system use interpolated transforms to achieve smooth movement of objects even when they are updated
    ///         in discrete time steps during fixed updates.
    ///     </para>
    ///     <para>
    ///         <b>Note:</b> When interpolation is enabled, changes made to the transform during a frame may not be immediately
    ///         reflected in the rendered position or orientation of the object. Instead, the rendered state is based on the
    ///         interpolated values calculated between fixed updates. This can lead to situations where rapid or frequent
    ///         changes to the transform appear delayed or smoothed out visually, which is intentional for achieving smooth
    ///         movement. If you require immediate visual feedback for a transform change (for example, for teleportation or
    ///         instant repositioning), use <see cref="SetTransformImmediate(Transform2D)" /> to bypass interpolation and
    ///         update the transform instantly.
    ///     </para>
    /// </remarks>
    public bool IsInterpolated
    {
        get => _isInterpolated;
        set
        {
            if (value == _isInterpolated)
            {
                return;
            }

            if (value)
            {
                TransformInterpolationId = TransformInterpolationSystem?.CreateTransform(this) ?? TransformInterpolationId.Invalid;
            }
            else
            {
                TransformInterpolationSystem?.DeleteTransform(TransformInterpolationId);
                TransformInterpolationId = TransformInterpolationId.Invalid;
            }

            _isInterpolated = value;
        }
    }

    /// <summary>
    ///     Gets the current state of interpolated 2D transform.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If interpolation is disabled or the interpolation system is unavailable, this property
    ///         returns the current transform. Otherwise, it retrieves the interpolated transform using the associated
    ///         interpolation system.
    ///     </para>
    ///     <para>
    ///         <see cref="InterpolatedTransform" /> may return different values from frame to frame, even if
    ///         <see cref="Transform" /> is not changed. This is because the interpolation system updates the interpolated
    ///         transform based on the time elapsed since the last fixed update, providing smooth transitions.
    ///     </para>
    /// </remarks>
    public Transform2D InterpolatedTransform
    {
        get
        {
            if (IsInterpolated is false || TransformInterpolationSystem is null)
            {
                return Transform;
            }

            return TransformInterpolationSystem.GetInterpolatedTransform(TransformInterpolationId);
        }
    }


    // TODO Should it return vector in global space taking into account transform hierarchy?
    /// <summary>
    ///     Unit vector in parent's local coordinate system (or global coordinate system if there is no parent) pointing
    ///     along X axis of coordinate system defined by this <see cref="Transform2DComponent" />.
    /// </summary>
    /// <remarks>
    ///     This property is useful to keep geometry logic relative to object's local coordinate system. If the object is
    ///     facing towards X axis in its own local coordinate system then after application of rotation this property gets
    ///     vector of where the object is facing in parent's local coordinate system (or global coordinate system if there is
    ///     no parent). It can be used to easily move an object along the direction it is facing by moving it along
    ///     <see cref="VectorX" />.
    /// </remarks>
    public Vector2 VectorX => (Matrix3x3.CreateRotation(Rotation) * Vector2.UnitX.Homogeneous).ToVector2();

    // TODO Should it return vector in global space taking into account transform hierarchy?
    /// <summary>
    ///     Unit vector in parent's local coordinate system (or global coordinate system if there is no parent) pointing along
    ///     Y axis of coordinate system defined by this <see cref="Transform2DComponent" />.
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
    ///     Sets the current transformation values immediately, bypassing any interpolation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method directly updates the transformation properties, including <see cref="Translation" />,
    ///         <see cref="Rotation" />, and <see cref="Scale" />, to the specified values. If interpolation is enabled, the
    ///         method also updates the interpolation state to reflect the new transform immediately.
    ///     </para>
    ///     <para>
    ///         Use this method when you need to apply a transformation instantly, such as for teleportation or instant
    ///         repositioning.
    ///     </para>
    /// </remarks>
    /// <param name="transform">
    ///     The <see cref="Transform2D" /> instance containing the translation, rotation, and scale values to apply.
    /// </param>
    public void SetTransformImmediate(Transform2D transform)
    {
        Translation = transform.Translation;
        Rotation = transform.Rotation;
        Scale = transform.Scale;

        if (IsInterpolated && TransformInterpolationSystem is not null)
        {
            TransformInterpolationSystem.SetTransformImmediate(TransformInterpolationId, transform);
        }
    }

    /// <summary>
    ///     Creates 2D transformation matrix that represents this transform component.
    /// </summary>
    /// <returns>2D transformation matrix representing this transform component.</returns>
    public Matrix3x3 ToMatrix() => Matrix3x3.CreateTRS(Translation, Rotation, Scale);

    /// <summary>
    ///     Computes the 2D transformation matrix in world (global) coordinate space for the current transform hierarchy.
    /// </summary>
    /// <remarks>
    ///     This method traverses the hierarchy of entities from the root to the current entity, applying the transformations
    ///     defined by <see cref="Transform2DComponent" />  at each level. If the current entity is the root, the
    ///     transformation matrix is derived directly from the current entity's <see cref="Transform2DComponent" />. If the
    ///     entity or any ancestor in the hierarchy does not have a <see cref="Transform2DComponent" />, an identity matrix is
    ///     used for those entities.
    /// </remarks>
    /// <returns>
    ///     A <see cref="Matrix3x3" /> representing the 2D transformation matrix in world (global) coordinate space.
    /// </returns>
    public Matrix3x3 ComputeWorldTransformMatrix()
    {
        if (Entity.IsRoot)
        {
            return ToMatrix();
        }

        var parent = Entity.Parent;
        while (!parent.IsRoot && !parent.HasComponent<Transform2DComponent>())
        {
            parent = parent.Parent;
        }

        var parentTransform = parent.HasComponent<Transform2DComponent>()
            ? parent.GetComponent<Transform2DComponent>().ComputeWorldTransformMatrix()
            : Matrix3x3.Identity;

        return parentTransform * ToMatrix();
    }

    /// <summary>
    ///     Computes the 2D transformation matrix in world (global) coordinate space for the current transform hierarchy,
    ///     using interpolated transforms.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This method traverses the hierarchy of entities from the root to the current entity, applying the interpolated
    ///         transformations defined by <see cref="Transform2DComponent" /> at each level. If the current entity is the
    ///         root, the transformation matrix is derived directly from the current entity's interpolated transform. If the
    ///         entity or any ancestor in the hierarchy does not have a <see cref="Transform2DComponent" />, an identity matrix
    ///         is used for those entities.
    ///     </para>
    ///     <para>
    ///         Unlike <see cref="ComputeWorldTransformMatrix" />, which uses the exact transform state, this method uses
    ///         interpolated transform values, providing smooth transitions between discrete updates. This is particularly
    ///         useful for rendering, where smooth movement and animation are desired.
    ///     </para>
    /// </remarks>
    /// <returns>
    ///     A <see cref="Matrix3x3" /> representing the 2D transformation matrix in world (global) coordinate space,
    ///     computed from interpolated transforms.
    /// </returns>
    public Matrix3x3 ComputeInterpolatedWorldTransformMatrix()
    {
        if (Entity.IsRoot)
        {
            return InterpolatedTransform.ToMatrix();
        }

        var parent = Entity.Parent;
        while (!parent.IsRoot && !parent.HasComponent<Transform2DComponent>())
        {
            parent = parent.Parent;
        }

        var parentTransform = parent.HasComponent<Transform2DComponent>()
            ? parent.GetComponent<Transform2DComponent>().ComputeInterpolatedWorldTransformMatrix()
            : Matrix3x3.Identity;
        return parentTransform * InterpolatedTransform.ToMatrix();
    }

    /// <summary>
    ///     Serializes data of this instance of <see cref="Transform2DComponent" />.
    /// </summary>
    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteVector2("Translation", Translation);
        writer.WriteDouble("Rotation", Rotation);
        writer.WriteVector2("Scale", Scale);
        writer.WriteBool("IsInterpolated", IsInterpolated);
    }

    /// <summary>
    ///     Deserializes data of this instance of <see cref="Transform2DComponent" />.
    /// </summary>
    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        Translation = reader.ReadVector2("Translation");
        Rotation = reader.ReadDouble("Rotation");
        Scale = reader.ReadVector2("Scale");
        IsInterpolated = reader.ReadBool("IsInterpolated");
    }
}

internal sealed class Transform2DComponentFactory : ComponentFactory<Transform2DComponent>
{
    protected override Transform2DComponent CreateComponent(Entity entity) => new(entity);
}