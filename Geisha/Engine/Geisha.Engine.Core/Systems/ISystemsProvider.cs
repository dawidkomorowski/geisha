using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    public interface ISystemsProvider
    {
        IUpdatable GetSystemsUpdatableForScene(Scene scene);
    }
}