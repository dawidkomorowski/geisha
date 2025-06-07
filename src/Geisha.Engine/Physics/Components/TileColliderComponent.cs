using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

// TODO Add documentation for TileColliderComponent.
[ComponentId("Geisha.Engine.Physics.TileColliderComponent")]
public sealed class TileColliderComponent : Collider2DComponent
{
    internal TileColliderComponent(Entity entity) : base(entity)
    {
    }
}

internal sealed class TileColliderComponentFactory : ComponentFactory<TileColliderComponent>
{
    protected override TileColliderComponent CreateComponent(Entity entity) => new(entity);
}