using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel
{
    internal interface ISceneFactory
    {
        public Scene Create();
    }

    internal sealed class SceneFactory : ISceneFactory
    {
        private readonly IEnumerable<ISceneBehaviorFactory> _sceneBehaviorFactories;

        public SceneFactory(IEnumerable<ISceneBehaviorFactory> sceneBehaviorFactories)
        {
            _sceneBehaviorFactories = sceneBehaviorFactories;
        }

        public Scene Create() => new Scene(_sceneBehaviorFactories);
    }
}