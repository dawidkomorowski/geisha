using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface IVariableTimeStepSystem
    {
        int Priority { get; set; }

        void Update(Scene scene, double deltaTime);
    }
}