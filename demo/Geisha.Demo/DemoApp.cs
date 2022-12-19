using Geisha.Engine;
using System.Reflection;

namespace Geisha.Demo
{
    internal sealed class DemoApp : Game
    {
        public override string WindowTitle =>
            $"Geisha Engine Demo App {Assembly.GetAssembly(typeof(DemoApp))?.GetName().Version?.ToString(3)}";

        public override void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSceneBehaviorFactory<HelloSceneBehaviorFactory>();
        }
    }
}