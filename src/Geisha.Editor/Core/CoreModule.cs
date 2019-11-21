using Autofac;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.Core.Properties;

namespace Geisha.Editor.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Docking
            builder.RegisterType<CompositeDocumentFactory>().As<ICompositeDocumentFactory>().SingleInstance();

            // Properties
            builder.RegisterType<PropertiesTool>().As<Tool>().SingleInstance();
            builder.RegisterType<PropertiesViewModel>().AsSelf().SingleInstance();

            builder.RegisterInstance(EventBus.Default).As<IEventBus>();
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();
            builder.RegisterInstance(ViewRepository.Default).As<IViewRepository>();
        }
    }
}