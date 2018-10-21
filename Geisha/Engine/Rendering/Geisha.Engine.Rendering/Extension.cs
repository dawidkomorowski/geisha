using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Components.Definition;
using Geisha.Engine.Rendering.Configuration;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Engine Rendering";
        public string Description => "Provides rendering system and related components.";
        public string Category => "Rendering";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            // Assets
            containerBuilder.RegisterType<SpriteLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            containerBuilder.RegisterType<CameraDefinitionMapper>().As<IComponentDefinitionMapper>().SingleInstance();
            containerBuilder.RegisterType<SpriteRendererDefinitionMapper>().As<IComponentDefinitionMapper>().SingleInstance();
            containerBuilder.RegisterType<TextRendererDefinitionMapper>().As<IComponentDefinitionMapper>().SingleInstance();

            // Configuration
            containerBuilder.RegisterType<RenderingDefaultConfigurationFactory>().As<IDefaultConfigurationFactory>().SingleInstance();

            // Systems
            containerBuilder.RegisterType<RenderingSystem>().As<IVariableTimeStepSystem>().SingleInstance();
        }
    }
}