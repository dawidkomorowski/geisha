using Autofac;
using Benchmark.Benchmarks.Collision;
using Benchmark.Benchmarks.EmptyScene;
using Benchmark.Benchmarks.Entities;
using Benchmark.Benchmarks.Primitives;
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

            // Components
            componentsRegistry.RegisterComponentFactory<BulletBehaviorComponentFactory>();
            componentsRegistry.RegisterComponentFactory<CannonBehaviorComponentFactory>();
            componentsRegistry.RegisterComponentFactory<MovementBehaviorComponentFactory>();

            // Common
            componentsRegistry.AutofacContainerBuilder.RegisterType<EntityFactory>().As<IEntityFactory>().SingleInstance();

            // Benchmarks
            componentsRegistry.RegisterSceneBehaviorFactory<EmptySceneSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<EntitiesWithNoComponentsSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticPrimitivesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingPrimitivesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticSpritesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingSpritesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<AnimatedSpritesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingCollidersSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<EntitiesThroughputSceneBehaviorFactory>();
        }
    }
}