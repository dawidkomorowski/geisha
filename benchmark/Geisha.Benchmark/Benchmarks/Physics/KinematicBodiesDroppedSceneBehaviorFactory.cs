using System;
using System.Linq;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.Physics;

internal sealed class KinematicBodiesDroppedSceneBehaviorFactory : ISceneBehaviorFactory
{
    private const string SceneBehaviorName = "1580KinematicBodiesDropped";
    private readonly EntityFactory _entityFactory;

    public KinematicBodiesDroppedSceneBehaviorFactory(EntityFactory entityFactory)
    {
        _entityFactory = entityFactory;
    }

    public string BehaviorName => SceneBehaviorName;
    public SceneBehavior Create(Scene scene) => new KinematicBodiesDroppedSceneBehavior(scene, _entityFactory);

    private sealed class KinematicBodiesDroppedSceneBehavior : SceneBehavior
    {
        private readonly EntityFactory _entityFactory;

        public KinematicBodiesDroppedSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
        {
            _entityFactory = entityFactory;
        }

        public override string Name => SceneBehaviorName;

        protected override void OnLoaded()
        {
            const double gravity = -981.0;

            _entityFactory.CreateCamera(Scene).GetComponent<CameraComponent>();

            _entityFactory.CreateRectangleStaticBody(Scene, 0, -300, 1200, 50);
            _entityFactory.CreateRectangleStaticBody(Scene, -600, 0, 50, 600);
            _entityFactory.CreateRectangleStaticBody(Scene, 600, 0, 50, 600);

            for (var iy = 0; iy < 40; iy++)
            {
                for (var ix = 0; ix < 40; ix++)
                {
                    if (ix == 0 && iy % 2 == 1)
                    {
                        continue;
                    }

                    var x = -525 + ix * 25 - (25d / 2d * (iy % 2));
                    var y = -200 + iy * 30;

                    _entityFactory.CreateRectangleKinematicBodyWithGravity(Scene, x, y, 20, 20, gravity);
                }
            }
        }
    }
}