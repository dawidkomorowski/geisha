using Autofac;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Views.Infrastructure;

namespace Geisha.Editor.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Infrastructure
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();

            // Views
            builder.RegisterType<OpenFileDialogRequestFilePathService>().As<IRequestFilePathService>().SingleInstance();
        }
    }
}