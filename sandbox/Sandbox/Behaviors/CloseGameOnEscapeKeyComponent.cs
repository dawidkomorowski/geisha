using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class CloseGameOnEscapeKeyComponent : Component
    {
    }

    internal sealed class CloseGameOnEscapeKeyComponentFactory : ComponentFactory<CloseGameOnEscapeKeyComponent>
    {
        protected override CloseGameOnEscapeKeyComponent CreateComponent() => new CloseGameOnEscapeKeyComponent();
    }
}