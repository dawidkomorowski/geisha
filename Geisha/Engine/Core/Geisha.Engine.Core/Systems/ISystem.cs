using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    // TODO Consider introducing two interfaces IVariableUpdateSystem and IFixedUpdateSystem (or similar) and not force system to "support" both update methods
    public interface ISystem
    {
        int Priority { get; set; }
        UpdateMode UpdateMode { get; set; }

        void Update(Scene scene, double deltaTime);
        void FixedUpdate(Scene scene);
    }
}