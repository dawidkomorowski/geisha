using System.Collections.Generic;
using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.FileSystem;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<EngineManager>().As<IEngineManager>().SingleInstance();
            builder.RegisterType<GameTimeProvider>().As<IGameTimeProvider>().SingleInstance();

            // Assets
            builder.RegisterType<AssetStore>().As<IAssetStore>().SingleInstance();

            // Components
            builder.RegisterType<Transform2DComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<Transform3DComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Diagnostics
            builder.RegisterType<AggregatedDiagnosticInfoProvider>().As<IAggregatedDiagnosticInfoProvider>().SingleInstance()
                .OnActivated(e => e.Instance.Initialize(e.Context.Resolve<IEnumerable<IDiagnosticInfoProvider>>()));
            builder.RegisterType<CoreDiagnosticInfoProvider>().As<ICoreDiagnosticInfoProvider>().As<IDiagnosticInfoProvider>().SingleInstance();
            builder.RegisterType<PerformanceStatisticsProvider>().As<IPerformanceStatisticsProvider>().SingleInstance();
            builder.RegisterType<PerformanceStatisticsRecorder>().As<IPerformanceStatisticsRecorder>().SingleInstance();
            builder.RegisterType<PerformanceStatisticsStorage>().As<IPerformanceStatisticsStorage>().SingleInstance();

            // FileSystem
            builder.RegisterType<FileSystem.FileSystem>().As<IFileSystem>().SingleInstance();

            // GameLoop
            builder.RegisterType<GameLoop.GameLoop>().As<IGameLoop>().SingleInstance();
            builder.RegisterType<GameLoopSteps>().As<IGameLoopSteps>().SingleInstance();

            // SceneModel
            builder.RegisterType<ComponentFactoryProvider>().As<IComponentFactoryProvider>().SingleInstance()
                .OnActivated(e => e.Instance.Initialize(e.Context.Resolve<IEnumerable<IComponentFactory>>()));
            builder.RegisterType<EmptySceneBehaviorFactory>().As<ISceneBehaviorFactory>().SingleInstance();
            builder.RegisterType<SceneBehaviorFactoryProvider>().As<ISceneBehaviorFactoryProvider>().SingleInstance()
                .OnActivated(e => e.Instance.Initialize(e.Context.Resolve<IEnumerable<ISceneBehaviorFactory>>()));
            builder.RegisterType<SceneFactory>().As<ISceneFactory>().SingleInstance();
            builder.RegisterType<SceneLoader>().As<ISceneLoader>().SingleInstance();
            builder.RegisterType<SceneManager>().As<ISceneManager>().As<ISceneManagerInternal>().SingleInstance()
                .OnActivated(e => e.Instance.Initialize(e.Context.Resolve<IEnumerable<ISceneObserver>>()));
            builder.RegisterType<SceneSerializer>().As<ISceneSerializer>().SingleInstance();

            // Systems
            builder.RegisterType<BehaviorSystem>().As<IBehaviorGameLoopStep>().As<ISceneObserver>().SingleInstance();
        }
    }
}