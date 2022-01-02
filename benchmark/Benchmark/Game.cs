using Autofac;
using Benchmark.Benchmarks.Collision;
using Benchmark.Benchmarks.EmptyScene;
using Benchmark.Benchmarks.MovingSprites1000;
using Benchmark.Benchmarks.NoComponents10000Entities;
using Benchmark.Benchmarks.StaticSprites1000;
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
            componentsRegistry.RegisterSceneBehaviorFactory<NoComponents10000EntitiesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticSprites1000SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingSprites1000SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingCollidersSceneBehaviorFactory>();
        }
    }
}