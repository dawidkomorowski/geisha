namespace Geisha.Engine.E2EApp.EngineApiCanBeInjectedToCustomGameCode
{
    internal static class EngineApiCanBeInjectedToCustomGameCodeModule
    {
        public static void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSceneBehaviorFactory<EngineApiCanBeInjectedToCustomGameCodeSceneBehaviorFactory>();
            componentsRegistry.RegisterComponentFactory<EngineApiDependencyInjectionTestComponentFactory>();
            componentsRegistry.RegisterSystem<EngineApiDependencyInjectionTestSystem>();
        }
    }
}