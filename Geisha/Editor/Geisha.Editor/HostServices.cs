using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Common.Serialization;
using Geisha.Editor.Core;

namespace Geisha.Editor
{
    internal sealed class HostServices : IHostServices
    {
        public void Register(ContainerBuilder containerBuilder)
        {
            // Common
            containerBuilder.RegisterType<JsonSerializer>().As<IJsonSerializer>().SingleInstance();

            // Modules
            containerBuilder.RegisterModule<CoreModule>();
        }
    }
}