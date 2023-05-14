using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Sandbox
{
    internal sealed class ChangingTextComponent : BehaviorComponent
    {
        private TextRendererComponent? _textRenderer;
        private int _frameCounter = 0;

        public ChangingTextComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
            if (Entity.HasComponent<TextRendererComponent>())
            {
                _textRenderer = Entity.GetComponent<TextRendererComponent>();
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
            _frameCounter++;
            if (_textRenderer != null) _textRenderer.Text = $"Frame number: {_frameCounter}";
        }
    }

    internal sealed class ChangingTextComponentFactory : ComponentFactory<ChangingTextComponent>
    {
        protected override ChangingTextComponent CreateComponent(Entity entity) => new(entity);
    }
}