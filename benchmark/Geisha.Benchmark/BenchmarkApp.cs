using Autofac;
using Geisha.Benchmark.Benchmarks.Collision;
using Geisha.Benchmark.Benchmarks.EmptyScene;
using Geisha.Benchmark.Benchmarks.Entities;
using Geisha.Benchmark.Benchmarks.PrimitiveRendering;
using Geisha.Benchmark.Benchmarks.SpriteRendering;
using Geisha.Benchmark.Benchmarks.TextRendering;
using Geisha.Benchmark.Common;
using Geisha.Engine;

namespace Geisha.Benchmark
{
    internal sealed class BenchmarkApp : Game
    {
        public override string WindowTitle => "Geisha Engine Benchmark";

        public override void RegisterComponents(IComponentsRegistry componentsRegistry)
        {
            // Framework
            componentsRegistry.RegisterSceneBehaviorFactory<BenchmarkSceneBehaviorFactory>();
            componentsRegistry.RegisterSystem<BenchmarkSystem>();

            // Components
            componentsRegistry.RegisterComponentFactory<BulletBehaviorComponentFactory>();
            componentsRegistry.RegisterComponentFactory<CannonBehaviorComponentFactory>();
            componentsRegistry.RegisterComponentFactory<MovementBehaviorComponentFactory>();
            componentsRegistry.RegisterComponentFactory<ChangingTextComponentFactory>();

            // Common
            componentsRegistry.AutofacContainerBuilder.RegisterType<EntityFactory>().As<IEntityFactory>().SingleInstance();

            // Benchmarks
            componentsRegistry.RegisterSceneBehaviorFactory<EmptySceneSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<EntitiesWithNoComponentsSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticPrimitivesInViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticPrimitivesOutOfViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingPrimitivesInViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingPrimitivesOutOfViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticSpritesInViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticSpritesOutOfViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingSpritesInViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingSpritesOutOfViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpriteBatch10X1000SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpriteBatch100X100SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpriteBatch1000X10SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpriteBatch10000X1SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpritesInLayers10X1000SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpritesInLayers5X2000SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<SpritesInLayers2X5000SceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<AnimatedSpritesSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingCollidersSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<EntitiesThroughputSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticTextInViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<StaticTextOutOfViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingTextInViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<MovingTextOutOfViewSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<RotatingTextSceneBehaviorFactory>();
            componentsRegistry.RegisterSceneBehaviorFactory<ChangingTextSceneBehaviorFactory>();
        }
    }
}