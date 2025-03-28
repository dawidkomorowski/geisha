using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Physics;

public sealed class DynamicPhysicsEntityComponent : Component
{
    public DynamicPhysicsEntityComponent(Entity entity) : base(entity)
    {
    }
}

public sealed class DynamicPhysicsEntityComponentFactory : ComponentFactory<DynamicPhysicsEntityComponent>
{
    protected override DynamicPhysicsEntityComponent CreateComponent(Entity entity) => new(entity);
}