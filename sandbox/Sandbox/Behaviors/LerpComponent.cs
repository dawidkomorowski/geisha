using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    internal sealed class LerpComponent : BehaviorComponent
    {
        private readonly IDebugRenderer _renderer;
        private double _alpha;
        private double _sign = 1d;

        public LerpComponent(Entity entity, IDebugRenderer renderer) : base(entity)
        {
            _renderer = renderer;
        }

        public Vector2 StartPosition { get; set; }
        public Vector2 EndPosition { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);

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
            transform.Translation = Vector2.Lerp(StartPosition, EndPosition, _alpha);

            var transform1 = new Transform2D
            {
                Translation = new Vector2(-400, -500),
                Rotation = Angle.Deg2Rad(60),
                Scale = new Vector2(0.5, 0.5)
            };

            var transform2 = new Transform2D
            {
                Translation = new Vector2(-200, -200),
                Rotation = Angle.Deg2Rad(-80),
                Scale = new Vector2(2, 2)
            };

            var matrix1 = transform1.ToMatrix();
            var matrix2 = transform2.ToMatrix();
            var matrixLerp = Matrix3x3.Lerp(matrix1, matrix2, _alpha);
            var transformLerp = Transform2D.Lerp(transform1, transform2, _alpha).ToMatrix();

            var c1 = Color.FromArgb(255, 0, 0, 255);
            var c2 = Color.FromArgb(50, 255, 255, 0);
            var lerpColor = Color.Lerp(c1, c2, _alpha);

            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 0, 255, 0), matrix1);
            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 0, 255, 0), matrix2);
            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 255, 0, 0), matrixLerp);
            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), lerpColor, transformLerp);
        }
    }

    internal sealed class LerpComponentFactory : ComponentFactory<LerpComponent>
    {
        private readonly IDebugRenderer _renderer;

        public LerpComponentFactory(IDebugRenderer renderer)
        {
            _renderer = renderer;
        }

        protected override LerpComponent CreateComponent(Entity entity) => new(entity, _renderer);
    }
}