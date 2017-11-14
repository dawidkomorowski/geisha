using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Input.Components;
using Geisha.Framework.Input;

namespace Geisha.TestGame.Behaviors
{
    public class CloseGameOnEscapeKey : Behavior
    {
        private readonly IEngineManager _engineManager;

        public CloseGameOnEscapeKey(IEngineManager engineManager)
        {
            _engineManager = engineManager;
        }

        public override void OnFixedUpdate()
        {
            var inputComponent = Entity.GetComponent<InputComponent>();
            if (inputComponent.HardwareInput.KeyboardInput[Key.Escape]) _engineManager.ScheduleEngineShutdown();
        }
    }
}