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
        private readonly IReadOnlyCollection<ISceneBehaviorFactory> _sceneBehaviorFactories;

        public SceneModelFactory(ISceneBehaviorFactoryProvider sceneBehaviorFactoryProvider, IEnumerable<ISceneBehaviorFactory> sceneBehaviorFactories)
        {
            _sceneBehaviorFactoryProvider = sceneBehaviorFactoryProvider;
            _sceneBehaviorFactories = sceneBehaviorFactories.ToList().AsReadOnly();
        }

        public SceneModel Create(Scene scene) =>
            new SceneModel(
                scene,
                _sceneBehaviorFactories.Select(f => new SceneBehaviorName(f.BehaviorName)),
                _sceneBehaviorFactoryProvider
            );
    }
}