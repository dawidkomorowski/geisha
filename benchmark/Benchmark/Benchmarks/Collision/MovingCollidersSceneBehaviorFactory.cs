using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.Collision
{
    internal sealed class MovingCollidersSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingColliders";
        private readonly IEntityFactory _entityFactory;

        public MovingCollidersSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new MovingCollidersSceneBehavior(scene, _entityFactory);

        private sealed class MovingCollidersSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public MovingCollidersSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                var camera = _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();

                var width = camera.ViewRectangle.X * 3;
                var height = camera.ViewRectangle.Y * 3;

                var random = new Random(0);

                for (var i = 0; i < 300; i++)
                {
                    var x = width * random.NextDouble() - width / 2d;
                    var y = height * random.NextDouble() - height / 2d;

                    if (i % 2 == 0)
                    {
                        _entityFactory.CreateMovingCircleCollider(Scene, x, y, random);
                    }
                    else
                    {
                        _entityFactory.CreateMovingRectangleCollider(Scene, x, y, random);
                    }
                }
            }
        }
    }
}