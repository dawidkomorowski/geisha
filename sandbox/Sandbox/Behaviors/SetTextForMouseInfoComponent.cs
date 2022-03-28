using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    internal sealed class SetTextForMouseInfoComponent : BehaviorComponent
    {
        private InputComponent _inputComponent = null!;
        private TextRendererComponent _textRenderer = null!;

        public SetTextForMouseInfoComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
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

    internal sealed class SetTextForMouseInfoComponentFactory : ComponentFactory<SetTextForMouseInfoComponent>
    {
        protected override SetTextForMouseInfoComponent CreateComponent(Entity entity) => new SetTextForMouseInfoComponent(entity);
    }
}