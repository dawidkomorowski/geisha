using Benchmark.Benchmarks.EmptyScene;
using Benchmark.Benchmarks.NoComponents10000Entities;
using Benchmark.Benchmarks.StaticSprites1000;
using Geisha.Engine;

namespace Benchmark
{
    internal sealed class Game : IGame
    {
        public string WindowTitle => "Geisha Engine Benchmark";

        public void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            componentsRegistry.RegisterSceneBehaviorFactory<BenchmarkSceneBehaviorFactory>();
            componentsRegistry.RegisterSystem<BenchmarkSystem>();

            // Benchmarks
            componentsRegistry.RegisterSceneBehaviorFactory<EmptySceneBenchmarkSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<NoComponents10000EntitiesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticSprites1000SceneBehaviorFactory>();
        }
    }
}