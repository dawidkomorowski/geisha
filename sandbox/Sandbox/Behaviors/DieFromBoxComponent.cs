using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class DieFromBoxComponent : Component
    {
    }

    internal sealed class DieFromBoxComponentFactory : ComponentFactory<DieFromBoxComponent>
    {
        protected override DieFromBoxComponent CreateComponent() => new DieFromBoxComponent();
    }
}