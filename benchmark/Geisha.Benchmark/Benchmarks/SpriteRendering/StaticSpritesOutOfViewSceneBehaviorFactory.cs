﻿using System;
using Geisha.Benchmark.Common;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Benchmark.Benchmarks.SpriteRendering
{
    internal sealed class StaticSpritesOutOfViewSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "StaticSpritesOutOfView";
        private readonly EntityFactory _entityFactory;

        public StaticSpritesOutOfViewSceneBehaviorFactory(EntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new StaticSpritesOutOfViewSceneBehavior(scene, _entityFactory);

        private sealed class StaticSpritesOutOfViewSceneBehavior : SceneBehavior
        {
            private readonly EntityFactory _entityFactory;

            public StaticSpritesOutOfViewSceneBehavior(Scene scene, EntityFactory entityFactory) : base(scene)
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

                    _entityFactory.CreateStaticSprite(Scene, x, y);
                }
            }
        }
    }
}