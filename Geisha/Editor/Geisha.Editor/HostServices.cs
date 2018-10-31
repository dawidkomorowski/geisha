using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Editor.Core;

namespace Geisha.Editor
{
    internal sealed class HostServices : IHostServices
    {
        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<CoreModule>();
        }
    }
}