namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public sealed class EmptySceneConstructionScript : ISceneConstructionScript
    {
        public string Name => "Empty";

        public void Execute(Scene scene)
        {
        }
    }
}