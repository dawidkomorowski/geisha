using Geisha.Engine;
using Sandbox.Animation;
using Sandbox.Common;
using Sandbox.Physics;

namespace Sandbox;

public sealed class SandboxApp : Game
{
    public override string WindowTitle => "Geisha Engine Sandbox";

    public override void RegisterComponents(IComponentsRegistry componentsRegistry)
    {
        componentsRegistry.RegisterSingleInstance<CommonEntityFactory>();

        // Animation
        componentsRegistry.RegisterSceneBehaviorFactory<AnimationSandboxSceneBehaviorFactory>();
        componentsRegistry.RegisterComponentFactory<AnimationControllerComponentFactory>();

        // Physics
        componentsRegistry.RegisterSceneBehaviorFactory<PhysicsSandboxSceneBehaviorFactory>();
        componentsRegistry.RegisterComponentFactory<EntityControllerComponentFactory>();
        componentsRegistry.RegisterComponentFactory<LayoutControllerComponentFactory>();
        componentsRegistry.RegisterComponentFactory<DynamicPhysicsEntityComponentFactory>();
        componentsRegistry.RegisterComponentFactory<InfoComponentFactory>();
    }
}