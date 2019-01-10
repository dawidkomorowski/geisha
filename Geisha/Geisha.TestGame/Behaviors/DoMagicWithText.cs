using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Components;
using Geisha.Framework.Rendering;

namespace Geisha.TestGame.Behaviors
{
    [ComponentDefinition]
    public class DoMagicWithText : Behavior
    {
        private int _ticks = 0;
        private double _rate = 1;

        public override void OnFixedUpdate()
        {
            var time = (_ticks / 60.0) * _rate;

            var textRenderer = Entity.GetComponent<TextRenderer>();
            var transform = Entity.GetComponent<Transform>();

            textRenderer.FontSize = FontSize.FromPoints((Math.Sin(time * 2) + 1.1) * 40);
            //transform.Scale = new Vector3((Math.Sin(time) + 1.1) * 20, (Math.Sin(time) + 1.1) * 20, 1);

            _ticks++;
        }
    }
}