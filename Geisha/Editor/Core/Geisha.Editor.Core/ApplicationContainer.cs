using System.ComponentModel.Composition.Hosting;
using Geisha.Editor.Core.ViewModels.MainWindow;

namespace Geisha.Editor.Core
{
    public class ApplicationContainer
    {
        private readonly CompositionContainer _compositionContainer;

        public ApplicationContainer()
        {
            var applicationCatalog = new ApplicationCatalog();
            _compositionContainer = new CompositionContainer(applicationCatalog);
        }

        public MainViewModel CreateMainViewModel()
        {
            return _compositionContainer.GetExportedValue<MainViewModel>();
        }
    }
}