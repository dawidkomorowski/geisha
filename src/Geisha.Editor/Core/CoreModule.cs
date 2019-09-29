using Autofac;
using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CompositeDocumentFactory>().As<ICompositeDocumentFactory>().SingleInstance();

            builder.RegisterInstance(EventBus.Default).As<IEventBus>();
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
        }
    }
}