using Autofac;

namespace Geisha.Common.Extensibility
{
    public interface IHostServices
    {
        void Register(ContainerBuilder containerBuilder);
    }
}