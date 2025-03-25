using Geisha.Engine;
using Sandbox.Physics;

namespace Sandbox;

public sealed class SandboxApp : Game
{
    public override string WindowTitle => "Geisha Engine Sandbox";

    public override void RegisterComponents(IComponentsRegistry componentsRegistry)
    {
        componentsRegistry.RegisterSystem<SandboxSystem>();
        componentsRegistry.RegisterSceneBehaviorFactory<SandboxSceneBehaviorFactory>();
        componentsRegistry.RegisterSceneBehaviorFactory<PhysicsSandboxSceneBehaviorFactory>();
        componentsRegistry.RegisterComponentFactory<EntityControllerComponentFactory>();
    }
}