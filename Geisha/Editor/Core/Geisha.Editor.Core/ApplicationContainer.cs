using System;
using System.ComponentModel.Composition.Hosting;
using Geisha.Common.Logging;
using Geisha.Editor.Core.ViewModels.MainWindow;
using Geisha.Editor.Core.Views.MainWindow;

namespace Geisha.Editor.Core
{
    public class ApplicationContainer : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(ApplicationContainer));
        private ApplicationCatalog _applicationCatalog;
        private CompositionContainer _compositionContainer;
        private bool _disposed;

        public void Start()
        {
            ThrowIfDisposed();

            Log.Info("Starting application container.");

            Log.Info("Composing dependency graph.");
            _applicationCatalog = new ApplicationCatalog();
            _compositionContainer = new CompositionContainer(_applicationCatalog);
            var mainViewModel = _compositionContainer.GetExportedValue<MainViewModel>();
            Log.Info("Dependency graph composed successfully.");

            Log.Info("Creating application main window.");
            var mainWindow = new MainWindow {DataContext = mainViewModel};
            mainWindow.Show();

            Log.Info("Application container started successfully.");
        }

        public void Dispose()
        {
            if (_disposed) return;

            Log.Info("Disposing application container.");
            _disposed = true;
            _applicationCatalog.Dispose();
            _compositionContainer.Dispose();
            Log.Info("Application container disposed.");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ApplicationContainer));
        }
    }
}