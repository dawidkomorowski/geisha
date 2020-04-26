using Autofac;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Components.Serialization;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Audio
{
    /// <summary>
    ///     Provides audio system and related components.
    /// </summary>
    public sealed class AudioModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Assets
            builder.RegisterType<SoundAssetDiscoveryRule>().As<IAssetDiscoveryRule>().SingleInstance();
            builder.RegisterType<SoundManagedAssetFactory>().As<IManagedAssetFactory>().SingleInstance();

            // Components
            builder.RegisterType<SerializableAudioSourceComponentMapper>().As<ISerializableComponentMapper>().SingleInstance();

            // Systems
            builder.RegisterType<AudioSystem>().As<IAudioSystem>().SingleInstance();
        }
    }
}