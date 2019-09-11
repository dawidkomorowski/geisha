using Autofac;

namespace Geisha.Editor.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Infrastructure
            builder.RegisterInstance(EventBus.Default).As<IEventBus>();
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();

            // Views
            builder.RegisterType<OpenFileDialogService>().As<IOpenFileDialogService>().SingleInstance();
        }
    }
}