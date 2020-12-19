using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace TestGame.Behaviors
{
    [SerializableComponent]
    public sealed class SetTextForMouseInfoComponent : BehaviorComponent
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
            var mouseInput = _inputComponent.HardwareInput.MouseInput;
            var mousePosition = mouseInput.Position;
            var mouseScrollDelta = mouseInput.ScrollDelta;
            _textRenderer.Text = $"Mouse state({mousePosition})(S{mouseScrollDelta}): ";

            if (mouseInput.LeftButton)
            {
                _textRenderer.Text += "Left";
            }

            if (mouseInput.MiddleButton)
            {
                _textRenderer.Text += "Middle";
            }

            if (mouseInput.RightButton)
            {
                _textRenderer.Text += "Right";
            }

            if (mouseInput.XButton1)
            {
                _textRenderer.Text += "X1";
            }

            if (mouseInput.XButton2)
            {
                _textRenderer.Text += "X2";
            }
        }
    }
}