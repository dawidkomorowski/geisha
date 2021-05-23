using System;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    public sealed class SetTextForCurrentKeyComponent : BehaviorComponent
    {
        private TextRendererComponent _textRenderer = null!;
        private InputComponent _inputComponent = null!;

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            _textRenderer = Entity.GetComponent<TextRendererComponent>();
            _inputComponent = Entity.GetComponent<InputComponent>();
        }

        public override void OnFixedUpdate()
        {
            var keyboardInput = _inputComponent.HardwareInput.KeyboardInput;

            var key = Enum.GetValues(typeof(Key)).Cast<Key>().FirstOrDefault(k => keyboardInput[k]);
            _textRenderer.Text = keyboardInput[key] ? $"Keyboard state ({key})" : $"Keyboard state (no key pressed)";
        }
    }
}