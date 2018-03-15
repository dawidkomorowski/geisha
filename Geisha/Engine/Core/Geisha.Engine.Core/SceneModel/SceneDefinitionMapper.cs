using System.ComponentModel.Composition;

namespace Geisha.Engine.Core.SceneModel
{
    public interface ISceneDefinitionMapper
    {
        SceneDefinition ToDefinition(Scene scene);
        Scene FromDefinition(SceneDefinition sceneDefinition);
    }

    [Export(typeof(ISceneDefinitionMapper))]
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