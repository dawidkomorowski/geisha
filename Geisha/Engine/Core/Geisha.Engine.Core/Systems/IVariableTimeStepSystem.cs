using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface IVariableTimeStepSystem
    {
        // TODO Is it best way for ordering systems?
        int Priority { get; set; }

        void Update(Scene scene, double deltaTime);
    }
}