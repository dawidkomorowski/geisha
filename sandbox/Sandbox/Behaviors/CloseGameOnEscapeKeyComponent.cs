using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Sandbox.Behaviors
{
    [SerializableComponent]
    public class CloseGameOnEscapeKeyComponent : IComponent
    {
        public ComponentId ComponentId { get; } = new ComponentId("Sandbox.Behaviors.CloseGameOnEscapeKeyComponent");
    }
}