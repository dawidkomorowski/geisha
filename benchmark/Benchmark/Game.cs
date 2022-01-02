using Autofac;
using Benchmark.Benchmarks.Collision;
using Benchmark.Benchmarks.EmptyScene;
using Benchmark.Benchmarks.EntitiesWithNoComponents;
using Benchmark.Benchmarks.Sprites;
using Benchmark.Common;
using Geisha.Engine;

namespace Benchmark
{
    internal sealed class Game : IGame
    {
        public string WindowTitle => "Geisha Engine Benchmark";

        public void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            // Framework
            componentsRegistry.RegisterSceneBehaviorFactory<BenchmarkSceneBehaviorFactory>();
            componentsRegistry.RegisterSystem<BenchmarkSystem>();

            // Common
            componentsRegistry.AutofacContainerBuilder.RegisterType<EntityFactory>().As<IEntityFactory>().SingleInstance();

            // Benchmarks
            componentsRegistry.RegisterSceneBehaviorFactory<EmptySceneBenchmarkSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<EntitiesWithNoComponentsSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticSpritesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingSpritesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingCollidersSceneBehaviorFactory>();
        }
    }
}