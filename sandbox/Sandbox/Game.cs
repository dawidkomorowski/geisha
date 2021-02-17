using Geisha.Engine;

namespace Sandbox
{
    public sealed class Game : IGame
    {
        public string WindowTitle => "Geisha Engine Sandbox";

        public void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSystem<SandboxSystem>();
        }
    }
}