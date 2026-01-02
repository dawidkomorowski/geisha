namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal static class TestModule
    {
        public static void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSceneBehaviorFactory<TestSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<TestComponentFactory>();
            componentsRegistry.RegisterSystem<TestSystem>();
        }
    }
}