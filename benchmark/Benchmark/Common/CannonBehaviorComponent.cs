using System;
using System.Diagnostics;
using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Common
{
    internal sealed class CannonBehaviorComponent : BehaviorComponent
    {
        private TimeSpan _fireTimer = TimeSpan.Zero;
        private Transform2DComponent _cannonRotorTransform = null!;

        public Random? Random { get; set; }

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            Debug.Assert(Entity.Parent != null, "Entity.Parent != null");

            _cannonRotorTransform = Entity.Parent.GetComponent<Transform2DComponent>();
        }

        public override void OnFixedUpdate()
        {
            _cannonRotorTransform.Rotation -= GameTime.FixedDeltaTime.TotalSeconds * 5;
            _fireTimer += GameTime.FixedDeltaTime;

            if (_fireTimer > TimeSpan.FromSeconds(0.05))
            {
                _fireTimer = TimeSpan.Zero;
                FireBullet();
            }
        }

        private void FireBullet()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            Debug.Assert(Entity.Scene != null, "Entity.Scene != null");

            var entity = new Entity();
            Entity.Scene.AddEntity(entity);

            var transform = TransformHierarchy.Calculate2DTransformationMatrix(Entity);

            entity.AddComponent(new Transform2DComponent
            {
                Translation = (transform * new Vector2(0, 20).Homogeneous).ToVector2(),
                Rotation = 0,
                Scale = Vector2.One
            });

            Debug.Assert(Random != null, nameof(Random) + " != null");

            entity.AddComponent(new EllipseRendererComponent
            {
                Color = Color.FromArgb(1, Random.NextDouble(), Random.NextDouble(), Random.NextDouble()),
                Radius = 5,
                FillInterior = true,
                OrderInLayer = 2
            });

            entity.AddComponent(new BulletBehaviorComponent { Velocity = _cannonRotorTransform.VectorY * 200 });
        }
    }
}