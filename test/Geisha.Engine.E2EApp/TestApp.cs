using Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode;

namespace Geisha.Engine.E2EApp
{
    internal sealed class TestApp : Game
    {
        public override string WindowTitle => "Geisha Engine E2E Test Application";

        public override void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterComponentFactory<ExitTestAppComponentFactory>();

            EngineApiCanBeInjectedToCustomGameCodeModule.RegisterComponents(componentsRegistry);
        }
    }
}