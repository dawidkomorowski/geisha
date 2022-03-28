using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class DieFromBoxComponent : Component
    {
        public DieFromBoxComponent(Entity entity) : base(entity)
        {
        }
    }

    internal sealed class DieFromBoxComponentFactory : ComponentFactory<DieFromBoxComponent>
    {
        protected override DieFromBoxComponent CreateComponent(Entity entity) => new DieFromBoxComponent(entity);
    }
}