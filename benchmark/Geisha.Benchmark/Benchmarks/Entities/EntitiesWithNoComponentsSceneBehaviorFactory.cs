﻿using Geisha.Engine.Core.SceneModel;

namespace Geisha.Benchmark.Benchmarks.Entities
{
    internal sealed class EntitiesWithNoComponentsSceneBehaviorFactory : ISceneBehaviorFactory
    {
        private const string SceneBehaviorName = "EntitiesWithNoComponents";

        public string BehaviorName => SceneBehaviorName;

        public SceneBehavior Create(Scene scene) => new EntitiesNoComponentsSceneBehavior(scene);

        private sealed class EntitiesNoComponentsSceneBehavior : SceneBehavior
        {
            public EntitiesNoComponentsSceneBehavior(Scene scene) : base(scene)
            {
            }

            public override string Name => SceneBehaviorName;

            protected override void OnLoaded()
            {
                for (var i = 0; i < 10000; i++)
                {
                    Scene.CreateEntity();
                }
            }
        }
    }
}