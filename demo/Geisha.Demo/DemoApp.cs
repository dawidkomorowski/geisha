using Geisha.Engine;
using System.Reflection;
using Geisha.Demo.Common;
using Geisha.Demo.Screens.Hello;
using Geisha.Demo.Screens.Tmp;

namespace Geisha.Demo
{
    internal sealed class DemoApp : Game
    {
        public override string WindowTitle =>
            $"Geisha Engine Demo {Assembly.GetAssembly(typeof(DemoApp))?.GetName().Version?.ToString(3)}";

        public override void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSingleInstance<CommonScreenFactory>();
            componentsRegistry.RegisterSingleInstance<ScreenManager>();
            componentsRegistry.RegisterComponentFactory<MenuControlsComponentFactory>();

            // Hello
            componentsRegistry.RegisterSceneBehaviorFactory<HelloSceneBehaviorFactory>();

            // Tmp
            componentsRegistry.RegisterSceneBehaviorFactory<TmpSceneBehaviorFactory>();
        }
    }
}