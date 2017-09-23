using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Input;

namespace Geisha.TestGame.Behaviors
{
    public class SetTextForCurrentKey : Behavior
    {
        private string _initialText;

        public override void OnStart()
        {
            _initialText = Entity.GetComponent<TextRenderer>().Text;
        }

        public override void OnFixedUpdate()
        {
            var textRenderer = Entity.GetComponent<TextRenderer>();
            var inputComponent = Entity.GetComponent<InputComponent>();

            var key = Enum.GetValues(typeof(Key)).Cast<Key>().FirstOrDefault(k => inputComponent.HardwareInput.KeyboardInput[k]);

            textRenderer.Text = inputComponent.HardwareInput.KeyboardInput[key] ? key.ToString() : _initialText;
        }
    }
}