namespace Geisha.Engine.Core.SceneModel
{
    internal sealed class DefaultSceneBehavior : SceneBehavior
    {
        public const string Name = "DefaultSceneBehavior";

        public DefaultSceneBehavior(Scene scene) : base(scene)
        {
        }

        public override void OnLoaded()
        {
        }
    }

    internal sealed class DefaultSceneBehaviorFactory : ISceneBehaviorFactory
    {
        public string BehaviorName { get; } = DefaultSceneBehavior.Name;
        public SceneBehavior Create(Scene scene) => new DefaultSceneBehavior(scene);
    }
}