using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystem : IUpdatable
    {
        int Priority { get; set; }
        Scene Scene { get; set; }
    }
}