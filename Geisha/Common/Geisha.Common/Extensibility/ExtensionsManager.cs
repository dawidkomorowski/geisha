using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Geisha.Common.Logging;

namespace Geisha.Common.Extensibility
{
    public class ExtensionsManager
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ExtensionsManager));
        private ApplicationCatalog _applicationCatalog;
        private CompositionContainer _compositionContainer;
        private bool _disposed;

        public IEnumerable<IExtension> LoadExtensions()
        {
            ThrowIfDisposed();

            Log.Info("Starting MEF extensions container.");
            _applicationCatalog = new ApplicationCatalog();
            _compositionContainer = new CompositionContainer(_applicationCatalog);

            Log.Info("Loading extensions.");
            var extensions = _compositionContainer.GetExportedValues<IExtension>().ToList();
            foreach (var extension in extensions)
            {
                Log.Info($"Loaded extension: {extension.Name}@{extension.Version}");
            }

            Log.Info("Extensions loaded successfully.");

            return extensions;
        }

        public void Dispose()
        {
            if (_disposed) return;

            Log.Info("Disposing MEF extensions container.");
            _disposed = true;
            _compositionContainer.Dispose();
            _applicationCatalog.Dispose();
            Log.Info("MEF extensions container disposed.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ExtensionsManager));
        }
    }
}