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

            var translation1 = new Vector2(-400, -500);
            var rotation1 = Angle.Deg2Rad(60);
            var scale1 = new Vector2(0.5, 0.5);

            var translation2 = new Vector2(-200, -200);
            var rotation2 = Angle.Deg2Rad(-80);
            var scale2 = new Vector2(2, 2);

            var transform1 = Matrix3x3.CreateTranslation(translation1)
                             * Matrix3x3.CreateRotation(rotation1)
                             * Matrix3x3.CreateScale(scale1)
                             * Matrix3x3.Identity;
            var transform2 = Matrix3x3.CreateTranslation(translation2)
                             * Matrix3x3.CreateRotation(rotation2)
                             * Matrix3x3.CreateScale(scale2)
                             * Matrix3x3.Identity;
            var transformLerp = new Matrix3x3(
                GMath.Lerp(transform1.M11, transform2.M11, _alpha),
                GMath.Lerp(transform1.M12, transform2.M12, _alpha),
                GMath.Lerp(transform1.M13, transform2.M13, _alpha),
                GMath.Lerp(transform1.M21, transform2.M21, _alpha),
                GMath.Lerp(transform1.M22, transform2.M22, _alpha),
                GMath.Lerp(transform1.M23, transform2.M23, _alpha),
                GMath.Lerp(transform1.M31, transform2.M31, _alpha),
                GMath.Lerp(transform1.M32, transform2.M32, _alpha),
                GMath.Lerp(transform1.M33, transform2.M33, _alpha)
            );

            var transformLerp2 = Matrix3x3.CreateTranslation(Vector2.Lerp(translation1, translation2, _alpha))
                                 * Matrix3x3.CreateRotation(GMath.Lerp(rotation1, rotation2, _alpha))
                                 * Matrix3x3.CreateScale(Vector2.Lerp(scale1, scale2, _alpha))
                                 * Matrix3x3.Identity;

            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 0, 255, 0), transform1);
            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 0, 255, 0), transform2);
            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 255, 0, 0), transformLerp);
            _renderer.DrawRectangle(new AxisAlignedRectangle(100, 50), Color.FromArgb(255, 0, 0, 255), transformLerp2);
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