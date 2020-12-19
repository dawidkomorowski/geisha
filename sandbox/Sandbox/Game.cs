using Geisha.Engine;

namespace Sandbox
{
    public sealed class Game : IGame
    {
        public string WindowTitle => "Test Game";

        public void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSystem<TestSystem>();
            componentsRegistry.RegisterSceneConstructionScript<TestConstructionScript>();
        }
    }
}