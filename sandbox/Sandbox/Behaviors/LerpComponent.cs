using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class LerpComponent : BehaviorComponent
    {
        private double _alpha;
        private double _sign = 1d;

        public LerpComponent(Entity entity) : base(entity)
        {
        }

        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);

        public override void OnUpdate(GameTime gameTime)
        {
            var delta = gameTime.DeltaTime / Duration;
            _alpha += delta * _sign;

            if (_alpha > 1d)
            {
                _sign = -1d;
                _alpha -= _alpha - 1d;
            }

            if (_alpha < 0)
            {
                _sign = 1d;
                _alpha += 0 - _alpha;
            }

            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Translation = new Vector2(Interpolation.Lerp(StartPosition.X, EndPosition.X, _alpha),
                Interpolation.Lerp(StartPosition.Y, EndPosition.Y, _alpha));
        }
    }

    internal sealed class LerpComponentFactory : ComponentFactory<LerpComponent>
    {
        protected override LerpComponent CreateComponent(Entity entity) => new(entity);
    }
}