using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Common
{
    internal sealed class ChangingTextComponent : BehaviorComponent
    {
        private string _text = string.Empty;
        private int _length;
        private TextRendererComponent _textRenderer = null!;

        public ChangingTextComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
            _textRenderer = Entity.GetComponent<TextRendererComponent>();
            _text = _textRenderer.Text;
        }

        public override void OnFixedUpdate()
        {
            _length++;
            _length %= _text.Length;
            _textRenderer.Text = _text.Substring(0, _length);
        }
    }

    internal sealed class ChangingTextComponentFactory : ComponentFactory<ChangingTextComponent>
    {
        protected override ChangingTextComponent CreateComponent(Entity entity) => new(entity);
    }
}