namespace Geisha.Engine.E2EApp
{
    internal sealed class TestApp : IGame
    {
        public string WindowTitle => "Geisha Engine E2E Test Application";

        public void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSceneBehaviorFactory<MainSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<ExitTestAppComponentFactory>();
            componentsRegistry.RegisterComponentFactory<EngineApiDependencyInjectionTestComponentFactory>();
        }
    }
}