using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.TextRendering
{
    internal sealed class ChangingTextSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "ChangingText";
        private readonly IEntityFactory _entityFactory;

        public ChangingTextSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new ChangingTextSceneBehavior(scene, _entityFactory);

        internal sealed class ChangingTextSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public ChangingTextSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                    _entityFactory.CreateChangingText(Scene, x, y, random);
                }
            }
        }
    }
}