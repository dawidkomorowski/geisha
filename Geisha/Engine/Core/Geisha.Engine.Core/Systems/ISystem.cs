using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystem
    {
        int Priority { get; set; }
        UpdateMode UpdateMode { get; set; }

        void Update(Scene scene, double deltaTime);
        void FixedUpdate(Scene scene);
    }
}