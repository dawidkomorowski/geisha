using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface IFixedTimeStepSystem
    {
        int Priority { get; set; }

        void FixedUpdate(Scene scene);
    }
}