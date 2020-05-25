using System;
using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace TestGame.Behaviors
{
    [SerializableComponent]
    public class SetTextForCurrentKeyComponent : BehaviorComponent
    {
        private string _initialText = null!;
        private TextRendererComponent _textRenderer = null!;
        private InputComponent _inputComponent = null!;

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            _initialText = Entity.GetComponent<TextRendererComponent>().Text;
            _textRenderer = Entity.GetComponent<TextRendererComponent>();
            _inputComponent = Entity.GetComponent<InputComponent>();
        }

        public override void OnFixedUpdate()
        {
            var key = Enum.GetValues(typeof(Key)).Cast<Key>().FirstOrDefault(k => _inputComponent.HardwareInput.KeyboardInput[k]);
            _textRenderer.Text = _inputComponent.HardwareInput.KeyboardInput[key] ? key.ToString() : _initialText;
        }
    }
}