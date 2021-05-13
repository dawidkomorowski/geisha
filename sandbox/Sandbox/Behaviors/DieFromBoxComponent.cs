using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    public class DieFromBoxComponent : IComponent
    {
        public ComponentId ComponentId { get; } = new ComponentId("Sandbox.Behaviors.DieFromBoxComponent");
    }
}