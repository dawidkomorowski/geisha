using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    internal sealed class DoMagicWithTextComponent : BehaviorComponent
    {
        private const double Rate = 1;
        private int _ticks;

        public DoMagicWithTextComponent(Entity entity) : base(entity)
        {
        }

        public override void OnFixedUpdate()
        {
            var time = _ticks / 60.0 * Rate;

            var textRenderer = Entity.GetComponent<TextRendererComponent>();
            textRenderer.FontSize = FontSize.FromPoints((Math.Sin(time * 2) + 1.1) * 40);

            _ticks++;
        }
    }

    internal sealed class DoMagicWithTextComponentFactory : ComponentFactory<DoMagicWithTextComponent>
    {
        protected override DoMagicWithTextComponent CreateComponent(Entity entity) => new DoMagicWithTextComponent(entity);
    }
}