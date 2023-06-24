using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Sandbox
{
    internal sealed class ChangingTextComponent : BehaviorComponent
    {
        private Transform2DComponent? _transform;
        private TextRendererComponent? _textRenderer;
        private int _frameCounter = 0;

        public ChangingTextComponent(Entity entity) : base(entity)
        {
        }

        public override void OnStart()
        {
            if (Entity.HasComponent<Transform2DComponent>())
            {
                _transform = Entity.GetComponent<Transform2DComponent>();
            }

            if (Entity.HasComponent<TextRendererComponent>())
            {
                _textRenderer = Entity.GetComponent<TextRendererComponent>();
                _textRenderer.Text = $"Frame number: {_frameCounter}";
            }
        }

        public override void OnUpdate(GameTime gameTime)
        {
            _frameCounter++;
            if (_textRenderer != null) _textRenderer.Text = $"Frame number: {_frameCounter}";

            if (_transform != null)
            {
                _transform.Rotation += 2 * gameTime.DeltaTime.TotalSeconds;
            }
        }
    }

    internal sealed class ChangingTextComponentFactory : ComponentFactory<ChangingTextComponent>
    {
        protected override ChangingTextComponent CreateComponent(Entity entity) => new(entity);
    }
}