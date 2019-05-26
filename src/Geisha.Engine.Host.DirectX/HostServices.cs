using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Host.DirectX
{
    internal sealed class HostServices : IHostServices
    {
        private readonly IWindow _window;

        public HostServices(IWindow window)
        {
            _window = window;
        }

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstance(_window);
        }
    }
}