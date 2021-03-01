using System.Collections.Generic;

namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    /// Factory creating new instances of empty <see cref="Scene"/>.
    /// </summary>
    public interface ISceneFactory
    {
        /// <summary>
        /// Creates new instance of empty <see cref="Scene"/>.
        /// </summary>
        /// <returns>New instance of empty <see cref="Scene"/>.</returns>
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