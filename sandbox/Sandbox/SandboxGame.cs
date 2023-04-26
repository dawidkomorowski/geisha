using Geisha.Engine;

namespace Sandbox
{
    public sealed class SandboxGame : Game
    {
        public override string WindowTitle => "Geisha Engine Sandbox";

        public override void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSystem<SandboxSystem>();
            componentsRegistry.RegisterSceneBehaviorFactory<SandboxSceneBehaviorFactory>();
        }
    }
}