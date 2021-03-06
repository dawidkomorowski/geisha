﻿using Autofac;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.StartUpTasks;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    /// <summary>
    ///     Provides core engine infrastructure like game loop, scene management, assets management, entity-component-system
    ///     architecture building blocks.
    /// </summary>
    public sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EngineManager>().As<IEngineManager>().SingleInstance();
            builder.RegisterType<GameLoop>().As<IGameLoop>().SingleInstance();
            builder.RegisterType<GameTimeProvider>().As<IGameTimeProvider>().SingleInstance();

            // Assets
            builder.RegisterType<AssetStore>().As<IAssetStore>().SingleInstance();

            // Components
            builder.RegisterType<Transform2DComponentFactory>().As<IComponentFactory>().SingleInstance();
            builder.RegisterType<Transform3DComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Diagnostics
            builder.RegisterType<AggregatedDiagnosticInfoProvider>().As<IAggregatedDiagnosticInfoProvider>().As<IAggregatedDiagnosticInfoRegistry>()
                .SingleInstance();
            builder.RegisterType<CoreDiagnosticInfoProvider>().As<ICoreDiagnosticInfoProvider>().As<IDiagnosticInfoProvider>().SingleInstance();
            builder.RegisterType<PerformanceStatisticsProvider>().As<IPerformanceStatisticsProvider>().SingleInstance();
            builder.RegisterType<PerformanceStatisticsRecorder>().As<IPerformanceStatisticsRecorder>().SingleInstance();
            builder.RegisterType<PerformanceStatisticsStorage>().As<IPerformanceStatisticsStorage>().SingleInstance();

            // SceneModel
            builder.RegisterType<ComponentFactoryProvider>().As<IComponentFactoryProvider>().SingleInstance();
            builder.RegisterType<EmptySceneBehaviorFactory>().As<ISceneBehaviorFactory>().SingleInstance();
            builder.RegisterType<SceneBehaviorFactoryProvider>().As<ISceneBehaviorFactoryProvider>().SingleInstance();
            builder.RegisterType<SceneFactory>().As<ISceneFactory>().SingleInstance();
            builder.RegisterType<SceneLoader>().As<ISceneLoader>().SingleInstance();
            builder.RegisterType<SceneManager>().As<ISceneManager>().As<ISceneManagerForGameLoop>().SingleInstance();
            builder.RegisterType<SceneSerializer>().As<ISceneSerializer>().SingleInstance();

            // StartUpTasks
            builder.RegisterType<LoadStartUpSceneStartUpTask>().AsSelf().SingleInstance();
            builder.RegisterType<RegisterAssetsAutomaticallyStartUpTask>().AsSelf().SingleInstance();
            builder.RegisterType<RegisterDiagnosticInfoProvidersStartUpTask>().AsSelf().SingleInstance();

            // Systems
            builder.RegisterType<BehaviorSystem>().As<IBehaviorSystem>().SingleInstance();
            builder.RegisterType<EngineSystems>().As<IEngineSystems>().SingleInstance();
            builder.RegisterType<EntityDestructionSystem>().As<IEntityDestructionSystem>().SingleInstance();
        }
    }
}