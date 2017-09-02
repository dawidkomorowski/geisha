using System;
using System.ComponentModel.Composition.Hosting;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core
{
    public class EngineContainer : IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(EngineContainer));
        private ApplicationCatalog _applicationCatalog;
        private CompositionContainer _compositionContainer;

        public IEngine Engine { get; private set; }

        public void Dispose()
        {
            Log.Info("Disposing engine container.");
            _applicationCatalog?.Dispose();
            _compositionContainer?.Dispose();
            Log.Info("Engine container disposed.");
        }

        public void Start()
        {
            Log.Info("Starting engine container.");

            Log.Info("Composing dependency graph.");
            _applicationCatalog = new ApplicationCatalog();
            _compositionContainer = new CompositionContainer(_applicationCatalog);
            Engine = _compositionContainer.GetExportedValue<IEngine>();
            Log.Info("Dependency graph composed successfully.");

            Log.Info("Engine container started successfully.");
        }
    }
}