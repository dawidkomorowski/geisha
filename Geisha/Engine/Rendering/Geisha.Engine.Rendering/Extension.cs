using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Configuration;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Components.Serialization;
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
            containerBuilder.RegisterType<SpriteAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            containerBuilder.RegisterType<SpriteLoader>().As<IAssetLoader>().SingleInstance();
            containerBuilder.RegisterType<TextureAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            containerBuilder.RegisterType<TextureLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            containerBuilder.RegisterType<SerializableCameraComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            containerBuilder.RegisterType<SerializableSpriteRendererComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();
            containerBuilder.RegisterType<SerializableTextRendererComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Configuration
            containerBuilder.RegisterType<RenderingDefaultConfigurationFactory>().As<IDefaultConfigurationFactory>().SingleInstance();

            // Systems
            containerBuilder.RegisterType<RenderingSystem>().As<IVariableTimeStepSystem>().SingleInstance();
        }
    }
}