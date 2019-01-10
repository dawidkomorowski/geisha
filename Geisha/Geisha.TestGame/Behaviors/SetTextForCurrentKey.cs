using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Input;

namespace Geisha.TestGame.Behaviors
{
    [ComponentDefinition]
    public class SetTextForCurrentKey : Behavior
    {
        private string _initialText;
        private TextRenderer _textRenderer;
        private InputComponent _inputComponent;

        public override void OnStart()
        {
            _initialText = Entity.GetComponent<TextRenderer>().Text;
            _textRenderer = Entity.GetComponent<TextRenderer>();
            _inputComponent = Entity.GetComponent<InputComponent>();
        }

        public override void OnFixedUpdate()
        {
            var key = Enum.GetValues(typeof(Key)).Cast<Key>().FirstOrDefault(k => _inputComponent.HardwareInput.KeyboardInput[k]);
            _textRenderer.Text = _inputComponent.HardwareInput.KeyboardInput[key] ? key.ToString() : _initialText;
        }
    }
}