using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class DoMagicWithTextComponent : BehaviorComponent
    {
        private int _ticks = 0;
        private double _rate = 1;

        public override void OnFixedUpdate()
        {
            var time = (_ticks / 60.0) * _rate;

            var textRenderer = Entity.GetComponent<TextRendererComponent>();
            var transform = Entity.GetComponent<TransformComponent>();

            textRenderer.FontSize = FontSize.FromPoints((Math.Sin(time * 2) + 1.1) * 40);
            //transform.Scale = new Vector3((Math.Sin(time) + 1.1) * 20, (Math.Sin(time) + 1.1) * 20, 1);

            _ticks++;
        }
    }
}