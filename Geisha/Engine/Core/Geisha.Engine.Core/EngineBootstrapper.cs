using System.ComponentModel.Composition.Hosting;

namespace Geisha.Engine.Core
{
    public class EngineBootstrapper
    {
        public IEngine CreateNewEngine()
        {
            var applicationCatalog = new ApplicationCatalog();
            var compositionContainer = new CompositionContainer(applicationCatalog);
            return compositionContainer.GetExportedValue<IEngine>();
        }
    }
}