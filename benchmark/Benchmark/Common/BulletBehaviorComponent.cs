using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Common
{
    internal sealed class BulletBehaviorComponent : BehaviorComponent
    {
        private TimeSpan _lifeTime = TimeSpan.Zero;
        private Transform2DComponent _transform2DComponent = null!;

        public BulletBehaviorComponent(Entity entity) : base(entity)
        {
        }

        public Vector2 Velocity { get; set; }

        public override void OnStart()
        {
            _transform2DComponent = Entity.GetComponent<Transform2DComponent>();
        }

        public override void OnFixedUpdate()
        {
            _transform2DComponent.Translation += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
            _lifeTime += GameTime.FixedDeltaTime;

            if (_lifeTime > TimeSpan.FromSeconds(2))
            {
                Entity.RemoveAfterFixedTimeStep();
            }
        }
    }

    internal sealed class BulletBehaviorComponentFactory : ComponentFactory<BulletBehaviorComponent>
    {
        protected override BulletBehaviorComponent CreateComponent(Entity entity) => new BulletBehaviorComponent(entity);
    }
}