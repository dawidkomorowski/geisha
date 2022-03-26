using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class CloseGameOnEscapeKeyComponent : Component
    {
        public CloseGameOnEscapeKeyComponent(Entity entity) : base(entity)
        {
        }
    }

    internal sealed class CloseGameOnEscapeKeyComponentFactory : ComponentFactory<CloseGameOnEscapeKeyComponent>
    {
        protected override CloseGameOnEscapeKeyComponent CreateComponent(Entity entity) => new CloseGameOnEscapeKeyComponent(entity);
    }
}