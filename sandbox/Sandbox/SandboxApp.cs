using Geisha.Engine;
using Sandbox.Common;
using Sandbox.Physics;

namespace Sandbox;

public sealed class SandboxApp : Game
{
    public override string WindowTitle => "Geisha Engine Sandbox";

    public override void RegisterComponents(IComponentsRegistry componentsRegistry)
    {
        componentsRegistry.RegisterSingleInstance<CommonEntityFactory>();

        // Physics
        componentsRegistry.RegisterSceneBehaviorFactory<PhysicsSandboxSceneBehaviorFactory>();
        componentsRegistry.RegisterComponentFactory<EntityControllerComponentFactory>();
    }
}