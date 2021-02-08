using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.SceneModel
{
    public interface IGenericSceneBehaviorFactory
    {
        SceneBehavior Create(string behaviorName, Scene scene);
    }

    internal sealed class GenericSceneBehaviorFactory : IGenericSceneBehaviorFactory
    {
        private readonly Dictionary<string, ISceneBehaviorFactory> _sceneBehaviorFactories;

        public GenericSceneBehaviorFactory(IEnumerable<ISceneBehaviorFactory> sceneBehaviorFactories)
        {
            // TODO Check duplicated names?
            _sceneBehaviorFactories = sceneBehaviorFactories.ToDictionary(f => f.BehaviorName);
        }

        public SceneBehavior Create(string behaviorName, Scene scene)
        {
            return _sceneBehaviorFactories[behaviorName].Create(scene);
        }
    }
}