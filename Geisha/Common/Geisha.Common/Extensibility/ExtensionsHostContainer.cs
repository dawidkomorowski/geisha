using System;
using Autofac;
using Geisha.Common.Logging;

namespace Geisha.Common.Extensibility
{
    public sealed class ExtensionsHostContainer<TCompositionRoot> : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ExtensionsHostContainer<TCompositionRoot>));
        private readonly ExtensionsManager _extensionsManager;
        private readonly IContainer _container;
        private readonly ILifetimeScope _lifetimeScope;

        public TCompositionRoot CompositionRoot { get; }

        public ExtensionsHostContainer() : this(new EmptyHostServices())
        {
        }

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