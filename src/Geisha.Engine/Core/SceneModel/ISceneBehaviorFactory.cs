namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Defines interface of factory creating instances of <see cref="SceneBehavior" /> with specific behavior name.
    /// </summary>
    /// <remarks>
    ///     Implementation of <see cref="ISceneBehaviorFactory" /> should be registered for each type of
    ///     <see cref="SceneBehavior" /> to make behavior available to the engine. Type of behavior created by the factory
    ///     is identified by <see cref="BehaviorName" />. <see cref="BehaviorName" /> of factory should be the same as
    ///     <see cref="SceneBehavior.Name" /> of created behaviors.
    /// </remarks>
    public interface ISceneBehaviorFactory
    {
        /// <summary>
        ///     Name of behavior created by factory. It must be the same as <see cref="SceneBehavior.Name" /> of created behavior
        ///     instances. It must be unique across all available behaviors.
        /// </summary>
        public string BehaviorName { get; }

        /// <summary>
        ///     Creates new instance of <see cref="SceneBehavior" /> for specified <paramref name="scene" />.
        /// </summary>
        /// <param name="scene"><see cref="Scene" /> instance for which the behavior is created.</param>
        /// <returns>New instance of <see cref="SceneBehavior" />.</returns>
        public SceneBehavior Create(Scene scene);
    }
}