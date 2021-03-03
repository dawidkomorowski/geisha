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
        public Scene Create() => new Scene();
    }
}