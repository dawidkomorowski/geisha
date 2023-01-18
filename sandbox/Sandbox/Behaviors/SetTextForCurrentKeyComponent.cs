using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    internal sealed class SetTextForCurrentKeyComponent : BehaviorComponent
    {
        private InputComponent _inputComponent = null!;
        private TextRendererComponent _textRenderer = null!;

        public SetTextForCurrentKeyComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
            _textRenderer = Entity.GetComponent<TextRendererComponent>();
            _inputComponent = Entity.GetComponent<InputComponent>();
        }

        public override void OnFixedUpdate()
        {
            var keyboardInput = _inputComponent.HardwareInput.KeyboardInput;

            var key = Enum.GetValues(typeof(Key)).Cast<Key>().FirstOrDefault(k => keyboardInput[k]);
            _textRenderer.Text = keyboardInput[key] ? $"Keyboard state ({key})" : "Keyboard state (no key pressed)";
        }
    }

    internal sealed class SetTextForCurrentKeyComponentFactory : ComponentFactory<SetTextForCurrentKeyComponent>
    {
        protected override SetTextForCurrentKeyComponent CreateComponent(Entity entity) => new(entity);
    }
}