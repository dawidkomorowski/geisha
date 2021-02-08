namespace Geisha.Engine.Core.SceneModel
{
    public abstract class SceneBehavior
    {
        public static SceneBehavior CreateDefault(Scene scene) => new DefaultSceneBehavior(scene);

        protected SceneBehavior(Scene scene)
        {
            Scene = scene;
        }

        protected Scene Scene { get; }

        public abstract void OnLoaded();

        private sealed class DefaultSceneBehavior : SceneBehavior
        {
            public DefaultSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override void OnLoaded()
            {
            }
        }
    }
}