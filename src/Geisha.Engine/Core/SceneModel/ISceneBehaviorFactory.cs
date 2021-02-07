namespace Geisha.Engine.Core.SceneModel
{
    public interface ISceneBehaviorFactory
    {
        public string BehaviorName { get; }
        public SceneBehavior Create(Scene scene);
    }
}