using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface IFixedTimeStepSystem
    {
        string Name { get; }
        void FixedUpdate(Scene scene);
    }
}