using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.Physics;

internal sealed class StaticAndKinematicBodiesSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "StaticAndKinematicBodies";
    private readonly IEntityFactory _entityFactory;

    public StaticAndKinematicBodiesSceneBehaviorFactory(IEntityFactory entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public string BehaviorName => SceneBehaviorName;

    public SceneBehavior Create(Scene scene) => new StaticAndKinematicBodiesSceneBehavior(scene, _entityFactory);

    private sealed class StaticAndKinematicBodiesSceneBehavior : SceneBehavior
    {
        private readonly IEntityFactory _entityFactory;

        public StaticAndKinematicBodiesSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
        {
            _entityFactory = entityFactory;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            var camera = _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();

            var width = camera.ViewRectangle.X * 10;
            var height = camera.ViewRectangle.Y * 10;

            var random = new Random(0);

            for (var i = 0; i < 100; i++)
            {
                var x = width * random.NextDouble() - width / 2d;
                var y = height * random.NextDouble() - height / 2d;

                if (i % 2 == 0)
                {
                    _entityFactory.CreateCircleKinematicBodyControlledByBehavior(Scene, x, y, random);
                }
                else
                {
                    _entityFactory.CreateRectangleKinematicBodyControlledByBehavior(Scene, x, y, random);
                }
            }

            for (var i = 0; i < 10000; i++)
            {
                var x = width * random.NextDouble() - width / 2d;
                var y = height * random.NextDouble() - height / 2d;

                if (i % 2 == 0)
                {
                    _entityFactory.CreateCircleStaticBody(Scene, x, y);
                }
                else
                {
                    _entityFactory.CreateRectangleStaticBody(Scene, x, y);
                }
            }
        }
    }
}