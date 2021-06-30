using Autofac;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
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
            builder.RegisterType<AudioSourceComponentFactory>().As<IComponentFactory>().SingleInstance();

            // Systems
            builder.RegisterType<AudioSystem>().As<IAudioSystem>().SingleInstance();
        }
    }
}