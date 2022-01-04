using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;

namespace Benchmark.Common
{
    internal sealed class BulletBehaviorComponent : BehaviorComponent
    {
        private TimeSpan _lifeTime = TimeSpan.Zero;
        private Transform2DComponent _transform2DComponent = null!;

        public Vector2 Velocity { get; set; }

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");

            _transform2DComponent = Entity.GetComponent<Transform2DComponent>();
        }

        public override void OnFixedUpdate()
        {
            _transform2DComponent.Translation += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
            _lifeTime += GameTime.FixedDeltaTime;

            if (_lifeTime > TimeSpan.FromSeconds(2))
            {
                Debug.Assert(Entity != null, nameof(Entity) + " != null");
                Entity.DestroyAfterFullFrame();
            }
        }
    }
}