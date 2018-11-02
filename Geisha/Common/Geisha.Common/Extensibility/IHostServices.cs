using Autofac;

namespace Geisha.Common.Extensibility
{
    /// <summary>
    ///     Represents services provided by the hosting environment. Typically it includes services specific to particular host
    ///     executable.
    /// </summary>
    /// <remarks>
    ///     Implement this interface and provide an instance to <see cref="ExtensionsHostContainer{TCompositionRoot}" />
    ///     to provide custom services of hosting environment to an application.
    /// </remarks>
    public interface IHostServices
    {
        /// <summary>
        ///     Performs registration of services in Autofac dependency injection container.
        /// </summary>
        /// <param name="containerBuilder">Autofac <see cref="ContainerBuilder" /> that allows to register services in container.</param>
        /// <remarks>Implement services registration in this method.</remarks>
        void Register(ContainerBuilder containerBuilder);
    }
}