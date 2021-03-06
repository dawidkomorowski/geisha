﻿using System;
using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    internal sealed class DoMagicWithTextComponent : BehaviorComponent
    {
        private int _ticks;
        private const double Rate = 1;

        public override void OnFixedUpdate()
        {
            var time = (_ticks / 60.0) * Rate;

            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var textRenderer = Entity.GetComponent<TextRendererComponent>();
            //var transform = Entity.GetComponent<Transform2DComponent>();

            textRenderer.FontSize = FontSize.FromPoints((Math.Sin(time * 2) + 1.1) * 40);
            //transform.Scale = new Vector2((Math.Sin(time) + 1.1) * 10, (Math.Sin(time) + 1.1) * 10);

            _ticks++;
        }
    }

    internal sealed class DoMagicWithTextComponentFactory : ComponentFactory<DoMagicWithTextComponent>
    {
        protected override DoMagicWithTextComponent CreateComponent() => new DoMagicWithTextComponent();
    }
}