namespace Geisha.Engine.Core.SceneModel
{
    public abstract class SceneBehavior
    {
        public static SceneBehavior CreateEmpty(Scene scene) => new EmptySceneBehavior(scene);

        protected SceneBehavior(Scene scene)
        {
            Scene = scene;
        }

        public abstract string Name { get; }

        protected Scene Scene { get; }

        protected internal abstract void OnLoaded();

        private sealed class EmptySceneBehavior : SceneBehavior
        {
            public EmptySceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name { get; } = string.Empty;

            protected internal override void OnLoaded()
            {
            }
        }
    }

    internal sealed class EmptySceneBehaviorFactory : ISceneBehaviorFactory
    {
        public string BehaviorName { get; } = string.Empty;
        public SceneBehavior Create(Scene scene) => SceneBehavior.CreateEmpty(scene);
    }
}