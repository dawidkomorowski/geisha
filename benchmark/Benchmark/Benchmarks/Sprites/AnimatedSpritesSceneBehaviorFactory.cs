﻿using System;
using Benchmark.Common;
using Geisha.Engine.Core.SceneModel;

namespace Benchmark.Benchmarks.Sprites
{
    internal sealed class AnimatedSpritesSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "AnimatedSprites";
        private readonly IEntityFactory _entityFactory;

        public AnimatedSpritesSceneBehaviorFactory(IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public string BehaviorName => SceneBehaviorName;
        public SceneBehavior Create(Scene scene) => new AnimatedSpritesSceneBehavior(scene, _entityFactory);

        private sealed class AnimatedSpritesSceneBehavior : SceneBehavior
        {
            private readonly IEntityFactory _entityFactory;

            public AnimatedSpritesSceneBehavior(Scene scene, IEntityFactory entityFactory) : base(scene)
            {
                _entityFactory = entityFactory;
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                _entityFactory.CreateCamera(Scene);

                var random = new Random(0);

                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < 50; j++)
                    {
                        const int size = 32;
                        const int margin = 4;

                        var x = 0 - margin / 2 - size / 2 + (-24 + j) * (size + margin);
                        var y = 0 - margin / 2 - size / 2 + (-9 + i) * (size + margin);

                        _entityFactory.CreateAnimatedSprite(Scene, x, y, random);
                    }
                }
            }
        }
    }
}