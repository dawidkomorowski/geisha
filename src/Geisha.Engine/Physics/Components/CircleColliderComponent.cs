using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Represents a 2D collider in the shape of a circle. Use this component to define circular geometry for static or
///     kinematic rigid bodies.
/// </summary>
/// <remarks>
///     <para>
///         To create a circular 2D static rigid body, an entity must have both a
///         <see cref="Core.Components.Transform2DComponent" /> and a <see cref="CircleColliderComponent" />.
///     </para>
///     <para>
///         To create a kinematic rigid body, see <see cref="KinematicRigidBody2DComponent" />.
///     </para>
/// </remarks>
[ComponentId("Geisha.Engine.Physics.CircleColliderComponent")]
public sealed class CircleColliderComponent : Collider2DComponent
{
    internal CircleColliderComponent(Entity entity) : base(entity)
    {
    }

    /// <summary>
    ///     Radius of circle.
    /// </summary>
    public double Radius { get; set; }

    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteDouble("Radius", Radius);
    }

    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        Radius = reader.ReadDouble("Radius");
    }
}

internal sealed class CircleColliderComponentFactory : ComponentFactory<CircleColliderComponent>
{
    protected override CircleColliderComponent CreateComponent(Entity entity) => new(entity);
}