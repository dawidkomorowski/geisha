using System;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Components.Serialization;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Audio
{
    internal sealed class Extension : IExtension
    {
        public string Name => "Geisha Engine Audio";
        public string Description => "Provides audio system and related components.";
        public string Category => "Audio";
        public string Author => "Geisha";
        public Version Version => typeof(Extension).Assembly.GetName().Version;

        public void Register(ContainerBuilder containerBuilder)
        {
            // Assets
            containerBuilder.RegisterType<SoundAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            containerBuilder.RegisterType<SoundLoader>().As<IAssetLoader>().SingleInstance();

            // Components
            containerBuilder.RegisterType<SerializableAudioSourceComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            containerBuilder.RegisterType<AudioSystem>().As<IVariableTimeStepSystem>().SingleInstance();
        }
    }
}