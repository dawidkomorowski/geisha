using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    public class CloseGameOnEscapeKeyComponent : IComponent
    {
        public ComponentId ComponentId { get; } = new ComponentId("Sandbox.Behaviors.CloseGameOnEscapeKeyComponent");
    }
}