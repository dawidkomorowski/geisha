using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.Physics;

internal sealed class CollisionResponseKinematicVsKinematicSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "CollisionResponseKinematicVsKinematic";
    private readonly IEntityFactory _entityFactory;

    public CollisionResponseKinematicVsKinematicSceneBehaviorFactory(IEntityFactory entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new CollisionResponseKinematicVsKinematicSceneBehavior(scene, _entityFactory);

    private sealed class CollisionResponseKinematicVsKinematicSceneBehavior : SceneBehavior
    {
        private readonly IEntityFactory _entityFactory;

        public CollisionResponseKinematicVsKinematicSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
        {
            _entityFactory = entityFactory;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();
            var random = new Random(0);

            _entityFactory.CreateRectangleStaticBody(Scene, 0, 0);
        }
    }
}