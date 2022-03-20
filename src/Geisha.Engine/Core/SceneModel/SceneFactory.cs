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
        private readonly IComponentFactoryProvider _componentFactoryProvider;

        public SceneFactory(IComponentFactoryProvider componentFactoryProvider)
        {
            _componentFactoryProvider = componentFactoryProvider;
        }

        public Scene Create() => new Scene(_componentFactoryProvider);
    }
}