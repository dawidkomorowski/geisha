using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.Physics;

internal sealed class MovingKinematicBodiesSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "MovingKinematicBodies";
    private readonly IEntityFactory _entityFactory;

    public MovingKinematicBodiesSceneBehaviorFactory(IEntityFactory entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public string BehaviorName => SceneBehaviorName;

    public SceneBehavior Create(Scene scene) => new MovingKinematicBodiesSceneBehavior(scene, _entityFactory);

    private sealed class MovingKinematicBodiesSceneBehavior : SceneBehavior
    {
        private readonly IEntityFactory _entityFactory;

        public MovingKinematicBodiesSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

            for (var i = 0; i < 1000; i++)
            {
                var x = width * random.NextDouble() - width / 2d;
                var y = height * random.NextDouble() - height / 2d;

                if (i % 2 == 0)
                {
                    _entityFactory.CreateMovingCircleKinematicBody(Scene, x, y, random);
                }
                else
                {
                    _entityFactory.CreateMovingRectangleKinematicBody(Scene, x, y, random);
                }
            }
        }
    }
}