using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Geisha.Common.Logging;

namespace Geisha.Common.Extensibility
{
    public sealed class ExtensionsManager : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ExtensionsManager));
        private ApplicationCatalog _applicationCatalog;
        private CompositionContainer _compositionContainer;
        private bool _disposed;
        private bool _extensionsLoaded;

        public IEnumerable<IExtension> LoadExtensions()
        {
            ThrowIfDisposed();
            ThrowIfExtensionsAlreadyLoaded();

            Log.Info("Creating MEF extensions container.");
            _applicationCatalog = new ApplicationCatalog();
            _compositionContainer = new CompositionContainer(_applicationCatalog);
            Log.Info("MEF extensions container created.");


            Log.Info("Loading extensions.");
            _extensionsLoaded = true;

            var extensions = _compositionContainer.GetExportedValues<IExtension>().ToList();
            foreach (var extension in extensions)
            {
                Log.Info($"Extension loaded: {extension.Format()}");
            }

            Log.Info("Extensions loaded successfully.");

            return extensions;
        }

        public void Dispose()
        {
            if (_disposed) return;

            Log.Info("Disposing MEF extensions container.");
            _disposed = true;
            _compositionContainer?.Dispose();
            _applicationCatalog?.Dispose();
            Log.Info("MEF extensions container disposed.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ExtensionsManager));
        }

        private void ThrowIfExtensionsAlreadyLoaded()
        {
            if (_extensionsLoaded) throw new InvalidOperationException($"Extensions were already loaded by this instance of {nameof(ExtensionsManager)}.");
        }
    }
}