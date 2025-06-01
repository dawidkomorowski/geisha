using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.Physics;

internal sealed class StaticBodiesSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "StaticBodies";
    private readonly EntityFactory _entityFactory;

    public StaticBodiesSceneBehaviorFactory(EntityFactory entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public string BehaviorName => SceneBehaviorName;

    public SceneBehavior Create(Scene scene) => new StaticBodiesSceneBehavior(scene, _entityFactory);

    private sealed class StaticBodiesSceneBehavior : SceneBehavior
    {
        private readonly EntityFactory _entityFactory;

        public StaticBodiesSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
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