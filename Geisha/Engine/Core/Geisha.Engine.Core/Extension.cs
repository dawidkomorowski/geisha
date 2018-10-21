using System;
using Autofac;
using Geisha.Common;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Core
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Engine Core";
        public string Description => "Provides core engine infrastructure.";
        public string Category => "Core";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Engine>().As<IEngine>().SingleInstance();
            containerBuilder.RegisterType<GameLoop>().As<IGameLoop>().SingleInstance();
            containerBuilder.RegisterType<EngineManager>().As<IEngineManager>().SingleInstance();
            containerBuilder.RegisterType<SystemsProvider>().As<ISystemsProvider>().SingleInstance();
            containerBuilder.RegisterType<GameTimeProvider>().As<IGameTimeProvider>().SingleInstance();
            containerBuilder.RegisterType<SceneManager>().As<ISceneManager>().SingleInstance();
            containerBuilder.RegisterType<CoreDiagnosticsInfoProvider>().As<ICoreDiagnosticsInfoProvider>().SingleInstance();
            containerBuilder.RegisterType<ConfigurationManager>().As<IConfigurationManager>().SingleInstance();
            containerBuilder.RegisterType<CoreDefaultConfigurationFactory>().As<IDefaultConfigurationFactory>().SingleInstance();
            containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            containerBuilder.RegisterType<SceneLoader>().As<ISceneLoader>().SingleInstance();
            containerBuilder.RegisterType<SceneDefinitionMapper>().As<ISceneDefinitionMapper>().SingleInstance();
            containerBuilder.RegisterType<EntityDefinitionMapper>().As<IEntityDefinitionMapper>().SingleInstance();
            containerBuilder.RegisterType<ComponentDefinitionMapperProvider>().As<IComponentDefinitionMapperProvider>().SingleInstance();
            containerBuilder.RegisterType<AssetStore>().As<IAssetStore>().SingleInstance();
            containerBuilder.RegisterType<AssetLoaderProvider>().As<IAssetLoaderProvider>().SingleInstance();
        }
    }
}