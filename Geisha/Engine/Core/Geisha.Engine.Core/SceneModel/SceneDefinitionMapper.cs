namespace Geisha.Engine.Core.SceneModel
{
    public interface ISceneDefinitionMapper
    {
        SceneDefinition ToDefinition(Scene scene);
        Scene FromDefinition(SceneDefinition sceneDefinition);
    }

    internal class SceneDefinitionMapper : ISceneDefinitionMapper
    {
        public SceneDefinition ToDefinition(Scene scene)
        {
            return new SceneDefinition();
        }

        public Scene FromDefinition(SceneDefinition sceneDefinition)
        {
            return new Scene();
        }
    }
}