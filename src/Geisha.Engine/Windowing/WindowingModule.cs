using Autofac;
using Geisha.Engine.Core.GameLoop;

namespace Geisha.Engine.Windowing;

internal sealed class WindowingModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<WindowingSystem>().As<IWindowingGameLoopStep>().SingleInstance();
    }
}