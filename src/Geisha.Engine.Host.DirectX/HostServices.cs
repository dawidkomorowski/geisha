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
            // Register host services
            containerBuilder.RegisterInstance(_window);

            // Register engine modules
            EngineModules.RegisterAll(containerBuilder);
        }
    }
}