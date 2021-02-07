namespace Geisha.Engine.Core.SceneModel
{
    public abstract class SceneBehavior
    {
        protected SceneBehavior(Scene scene)
        {
            Scene = scene;
        }

        protected Scene Scene { get; }

        public abstract void OnLoaded();
    }
}