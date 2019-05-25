using System;
using Autofac;
using Geisha.Common.Logging;

namespace Geisha.Common.Extensibility
{
    /// <summary>
    ///     Implements host for application composed of components, services and extensions using Autofac dependency injection
    ///     container and MEF extensions discovery.
    /// </summary>
    /// <typeparam name="TCompositionRoot">Type of composition root object, typically an entry point to hosted application.</typeparam>
    public sealed class ExtensionsHostContainer<TCompositionRoot> : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ExtensionsHostContainer<TCompositionRoot>));
        private readonly IContainer _container;
        private readonly ExtensionsManager _extensionsManager;
        private readonly ILifetimeScope _lifetimeScope;

        /// <summary>
        ///     Creates new instance of <see cref="ExtensionsHostContainer{TCompositionRoot}" /> with no services provided by the
        ///     host.
        /// </summary>
        public ExtensionsHostContainer() : this(new EmptyHostServices())
        {
        }

        /// <summary>
        ///     Creates new instance of <see cref="ExtensionsHostContainer{TCompositionRoot}" /> with specified services provided
        ///     by the host.
        /// </summary>
        /// <param name="hostServices">
        ///     Services provided by the host. Typically it includes services specific to particular host
        ///     executable.
        /// </param>
        public ExtensionsHostContainer(IHostServices hostServices)
        {
            _extensionsManager = new ExtensionsManager();
            var containerBuilder = new ContainerBuilder();

            hostServices.Register(containerBuilder);

            foreach (var extension in _extensionsManager.LoadExtensions())
            {
                extension.Register(containerBuilder);
            }

            Log.Info("Creating Autofac container.");
            _container = containerBuilder.Build();
            _lifetimeScope = _container.BeginLifetimeScope();
            Log.Info("Autofac container created.");

            Log.Info("Composing dependency graph.");
            CompositionRoot = _lifetimeScope.Resolve<TCompositionRoot>();
            Log.Info("Dependency graph composed successfully.");
        }

        /// <summary>
        ///     Composition root object instance. It is automatically created after
        ///     <see cref="ExtensionsHostContainer{TCompositionRoot}" /> is constructed.
        /// </summary>
        public TCompositionRoot CompositionRoot { get; }

        /// <summary>
        ///     Disposes Autofac dependency injection container and MEF extensions containers.
        /// </summary>
        public void Dispose()
        {
            Log.Info("Disposing Autofac container.");
            _lifetimeScope?.Dispose();
            _container?.Dispose();
            Log.Info("Autofac container disposed.");

            _extensionsManager?.Dispose();
        }

        private sealed class EmptyHostServices : IHostServices
        {
            public void Register(ContainerBuilder containerBuilder)
            {
            }
        }
    }
}