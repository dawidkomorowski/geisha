using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    // TODO Add documentation.
    public interface IFixedTimeStepSystem
    {
        string Name { get; }
        void FixedUpdate(Scene scene);
    }
}