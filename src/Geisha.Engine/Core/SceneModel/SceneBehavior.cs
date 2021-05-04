namespace Geisha.Engine.Core.SceneModel
{
    /// <summary>
    ///     Specifies behavior of <see cref="SceneModel.Scene" /> instance i.e. action to be executed when scene is loaded.
    ///     Implement derived class to customize this behavior.
    /// </summary>
    /// <remarks>
    ///     For derived classes there must be implemented and registered <see cref="ISceneBehaviorFactory" /> class to
    ///     make custom <see cref="SceneBehavior" /> available to the engine. Custom <see cref="SceneBehavior" /> and
    ///     <see cref="ISceneBehaviorFactory" /> implementations must use the same behavior name.
    ///     <para>Use this class to perform custom logic when scene was loaded and before it is run by game loop.</para>
    /// </remarks>
    public abstract class SceneBehavior
    {
        /// <summary>
        ///     Initializes new instance of <see cref="SceneBehavior" /> class for specified <paramref name="scene" />.
        /// </summary>
        /// <param name="scene"><see cref="SceneModel.Scene" /> instance for which this behavior is created.</param>
        protected SceneBehavior(Scene scene)
        {
            Scene = scene;
        }

        /// <summary>
        ///     Name of <see cref="SceneBehavior" />. It must be the same as <see cref="ISceneBehaviorFactory.BehaviorName" /> of
        ///     corresponding factory. It must be unique across all available behaviors.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        ///     <see cref="SceneModel.Scene" /> instance this behavior is created for.
        /// </summary>
        protected Scene Scene { get; }

        /// <summary>
        ///     Creates empty <see cref="SceneBehavior" />. This is default implementation of <see cref="SceneBehavior" /> that
        ///     performs no actions.
        /// </summary>
        /// <param name="scene"><see cref="SceneModel.Scene" /> instance for which empty behavior is created.</param>
        /// <returns>New instance of empty behavior.</returns>
        public static SceneBehavior CreateEmpty(Scene scene) => new EmptySceneBehavior(scene);

        /// <summary>
        ///     Executed when <see cref="SceneModel.Scene" /> completed loading and before it is run by the game loop.
        /// </summary>
        /// <remarks>
        ///     Use it to implement custom logic performed on loaded scene i.e. for setting up correct initial state
        ///     of <see cref="SceneModel.Scene" /> from the game save.
        /// </remarks>
        protected internal abstract void OnLoaded();

        private sealed class EmptySceneBehavior : SceneBehavior
        {
            public EmptySceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => string.Empty;

            protected internal override void OnLoaded()
            {
            }
        }
    }

    internal sealed class EmptySceneBehaviorFactory : ISceneBehaviorFactory
    {
        public string BehaviorName => string.Empty;
        public SceneBehavior Create(Scene scene) => SceneBehavior.CreateEmpty(scene);
    }
}