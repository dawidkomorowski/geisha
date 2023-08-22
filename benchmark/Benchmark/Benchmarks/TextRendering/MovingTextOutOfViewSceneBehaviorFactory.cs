﻿using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Benchmark.Benchmarks.TextRendering
{
    internal sealed class MovingTextOutOfViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "MovingTextOutOfView";
        private readonly IEntityFactory _entityFactory;

        public MovingTextOutOfViewSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new MovingTextOutOfViewSceneBehavior(scene, _entityFactory);

        private sealed class MovingTextOutOfViewSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public MovingTextOutOfViewSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
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

                for (var i = 0; i < 1000; i++)
                {
                    var x = width * random.NextDouble() - width / 2d;
                    var y = height * random.NextDouble() - height / 2d;

                    _entityFactory.CreateMovingText(Scene, x, y, random, true);
                }
            }
        }
    }
}