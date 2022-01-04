using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;

namespace Benchmark.Common
{
    internal sealed class MovementBehaviorComponent : BehaviorComponent
    {
        private double _time;
        private Vector2 _translation;
        private double _rotation;
        private Vector2 _scale;
        private Transform2DComponent _transform2D = null!;

        public double RandomFactor { get; set; }

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");

            _transform2D = Entity.GetComponent<Transform2DComponent>();

            _time = RandomFactor;
            _translation = _transform2D.Translation;
            _rotation = _transform2D.Rotation;
            _scale = _transform2D.Scale;
        }

        public override void OnFixedUpdate()
        {
            _time += GameTime.FixedDeltaTime.TotalSeconds;

            var translationVector = new Vector2(32 * Math.Sin(_time), 32 * Math.Cos(_time));
            var scaleValue = 0.5 + Math.Sin(_time);
            var scaleVector = new Vector2(scaleValue, scaleValue);

            _transform2D.Translation = _translation + translationVector;
            _transform2D.Rotation = _rotation + _time;
            _transform2D.Scale = _scale + scaleVector;
        }
    }
}