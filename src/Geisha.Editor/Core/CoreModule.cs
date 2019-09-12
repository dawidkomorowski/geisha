using Autofac;

namespace Geisha.Editor.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(EventBus.Default).As<IEventBus>();
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
        }
    }
}