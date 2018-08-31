using System;
using System.ComponentModel.Composition.Hosting;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core
{
    public sealed class EngineContainer : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(EngineContainer));
        private ApplicationCatalog _applicationCatalog;
        private CompositionContainer _compositionContainer;
        private readonly HostServices _hostServices;
        private IEngine _engine;
        private bool _disposed;

        public EngineContainer()
        {
            _hostServices = new HostServices();
        }

        public EngineContainer(HostServices hostServices)
        {
            _hostServices = hostServices;
        }

        public IEngine Engine
        {
            get
            {
                ThrowIfDisposed();
                return _engine;
            }
        }

        public void Start()
        {
            ThrowIfDisposed();

            Log.Info("Starting engine container.");

            _applicationCatalog = new ApplicationCatalog();
            _compositionContainer = new CompositionContainer(_applicationCatalog);
            _hostServices.RegisterServicesInContainer(_compositionContainer);
            Log.Info("Composing dependency graph.");
            _engine = _compositionContainer.GetExportedValue<IEngine>();
            Log.Info("Dependency graph composed successfully.");

            Log.Info("Engine container started successfully.");
        }

        public void Dispose()
        {
            if (_disposed) return;

            Log.Info("Disposing engine container.");
            _disposed = true;
            _engine = null;
            _compositionContainer.Dispose();
            _applicationCatalog.Dispose();
            Log.Info("Engine container disposed.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EngineContainer));
        }
    }
}