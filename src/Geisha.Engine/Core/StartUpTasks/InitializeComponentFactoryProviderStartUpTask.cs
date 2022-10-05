using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal sealed class InitializeComponentFactoryProviderStartUpTask : IStartUpTask
    {
        private readonly IComponentFactoryProvider _componentFactoryProvider;
        private readonly IEnumerable<IComponentFactory> _factories;

        public InitializeComponentFactoryProviderStartUpTask(IComponentFactoryProvider componentFactoryProvider, IEnumerable<IComponentFactory> factories)
        {
            _componentFactoryProvider = componentFactoryProvider;
            _factories = factories;
        }

        public void Run()
        {
            _componentFactoryProvider.Initialize(_factories);
        }
    }
}