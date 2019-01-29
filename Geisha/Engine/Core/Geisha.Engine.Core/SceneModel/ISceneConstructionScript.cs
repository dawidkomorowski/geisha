namespace Geisha.Engine.Core.SceneModel
{
    // TODO Add documentation.
    public interface ISceneConstructionScript
    {
        string Name { get; }
        void Execute(Scene scene);
    }
}