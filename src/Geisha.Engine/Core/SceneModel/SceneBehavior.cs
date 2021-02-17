namespace Geisha.Engine.Core.SceneModel
{
    public abstract class SceneBehavior
    {
        public static SceneBehavior CreateEmpty(Scene scene) => new EmptySceneBehavior(scene);

        protected SceneBehavior(Scene scene)
        {
            Scene = scene;
        }

        protected Scene Scene { get; }

        public abstract void OnLoaded();

        private sealed class EmptySceneBehavior : SceneBehavior
        {
            public EmptySceneBehavior(Scene scene) : base(scene)
            {
            }

            public override void OnLoaded()
            {
            }
        }
    }
}