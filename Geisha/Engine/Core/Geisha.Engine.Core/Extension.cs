using System;
using Autofac;
using Geisha.Common;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components.Definition;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Engine Core";

        public string Description =>
            "Provides core engine infrastructure like game loop, scene management, entity-component-system architecture building blocks.";

        public string Category => "Core";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            containerBuilder.RegisterType<Engine>().As<IEngine>().SingleInstance();
            containerBuilder.RegisterType<EngineManager>().As<IEngineManager>().SingleInstance();
            containerBuilder.RegisterType<GameLoop>().As<IGameLoop>().SingleInstance();
            containerBuilder.RegisterType<GameTimeProvider>().As<IGameTimeProvider>().SingleInstance();

            // Assets
            containerBuilder.RegisterType<AssetLoaderProvider>().As<IAssetLoaderProvider>().SingleInstance();
            containerBuilder.RegisterType<AssetStore>().As<IAssetStore>().SingleInstance();

            // Components
            containerBuilder.RegisterType<TransformDefinitionMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Configuration
            containerBuilder.RegisterType<ConfigurationManager>().As<IConfigurationManager>().SingleInstance();
            containerBuilder.RegisterType<CoreDefaultConfigurationFactory>().As<IDefaultConfigurationFactory>().SingleInstance();

            // Diagnostics
            containerBuilder.RegisterType<AggregatedDiagnosticInfoProvider>().As<IAggregatedDiagnosticInfoProvider>().As<IAggregatedDiagnosticInfoRegistry>()
                .SingleInstance();
            containerBuilder.RegisterType<CoreDiagnosticInfoProvider>().As<ICoreDiagnosticInfoProvider>().As<IDiagnosticInfoProvider>().SingleInstance();
            containerBuilder.RegisterType<PerformanceStatisticsProvider>().As<IPerformanceStatisticsProvider>().SingleInstance();
            containerBuilder.RegisterType<PerformanceStatisticsRecorder>().As<IPerformanceStatisticsRecorder>().SingleInstance();
            containerBuilder.RegisterType<PerformanceStatisticsStorage>().As<IPerformanceStatisticsStorage>().SingleInstance();

            // SceneModel
            containerBuilder.RegisterType<SceneLoader>().As<ISceneLoader>().SingleInstance();
            containerBuilder.RegisterType<SceneManager>().As<ISceneManager>().SingleInstance();
            containerBuilder.RegisterType<AutomaticComponentDefinitionMapper>().As<ISerializableComponentMapper>().SingleInstance();
            containerBuilder.RegisterType<SerializableComponentMapperProvider>().As<ISerializableComponentMapperProvider>().SingleInstance();
            containerBuilder.RegisterType<SerializableEntityMapper>().As<ISerializableEntityMapper>().SingleInstance();
            containerBuilder.RegisterType<SerializableSceneMapper>().As<ISerializableSceneMapper>().SingleInstance();

            // Systems
            containerBuilder.RegisterType<BehaviorSystem>().As<IVariableTimeStepSystem>().As<IFixedTimeStepSystem>().SingleInstance();
            containerBuilder.RegisterType<EntityDestructionSystem>().As<IFixedTimeStepSystem>().SingleInstance();
            containerBuilder.RegisterType<SystemsProvider>().As<ISystemsProvider>().SingleInstance();
        }
    }
}