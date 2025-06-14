﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     2D collider component in a shape of a circle.
/// </summary>
/// <remarks>
///     To create 2D static rigid body an entity needs to be composed of
///     <see cref="Core.Components.Transform2DComponent" /> and a collider component. Static rigid bodies offer much
///     greater performance than kinematic rigid bodies as a raw colliders. However, they are expensive to modify (change
///     dimensions or position).
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