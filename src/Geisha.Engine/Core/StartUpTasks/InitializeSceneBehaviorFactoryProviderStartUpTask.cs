using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.StartUpTasks
{
    internal sealed class InitializeSceneBehaviorFactoryProviderStartUpTask : IStartUpTask
    {
        private readonly ISceneBehaviorFactoryProviderInternal _sceneBehaviorFactoryProvider;
        private readonly IEnumerable<ISceneBehaviorFactory> _factories;

        public InitializeSceneBehaviorFactoryProviderStartUpTask(ISceneBehaviorFactoryProviderInternal sceneBehaviorFactoryProvider,
            IEnumerable<ISceneBehaviorFactory> factories)
        {
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;
            _factories = factories;
        }

        public void Run()
        {
            _sceneBehaviorFactoryProvider.Initialize(_factories);
        }
    }
}