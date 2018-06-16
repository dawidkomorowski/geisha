using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface IFixedTimeStepSystem
    {
        // TODO Is it best way for ordering systems?
        int Priority { get; set; }
        string Name { get; }
        void FixedUpdate(Scene scene);
    }
}