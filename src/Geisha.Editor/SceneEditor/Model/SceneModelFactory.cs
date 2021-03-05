using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public interface ISceneModelFactory
    {
        SceneModel Create(Scene scene);
    }

    public sealed class SceneModelFactory : ISceneModelFactory
    {
        private readonly ISceneBehaviorFactoryProvider _sceneBehaviorFactoryProvider;
        private readonly IEnumerable<SceneBehaviorName> _availableSceneBehaviors;

        public SceneModelFactory(ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider, IEnumerable<ISceneBehaviorFactory> sceneBehaviorFactories)
        {
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;
            _availableSceneBehaviors = sceneBehaviorFactories.Select(f => new SceneBehaviorName(f.BehaviorName)).ToList();
        }

        public SceneModel Create(Scene scene) =>
            new SceneModel(
                scene,
                _availableSceneBehaviors,
                _sceneBehaviorFactoryProvider
            );
    }
}