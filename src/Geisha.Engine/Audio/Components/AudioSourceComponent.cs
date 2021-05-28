using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Audio.Components
{
    // TODO It should be possible to specify whether to load sound into memory or to stream it from disk.
    // TODO For SFX it would be mainly from memory, while for music it could be from disk (saves a lot of memory, it is rare to play several tracks at the same time).
    /// <summary>
    ///     Audio source capable of playing a single sound.
    /// </summary>
    [ComponentId("Geisha.Engine.Audio.AudioSourceComponent")]
    public sealed class AudioSourceComponent : Component
    {
        /// <summary>
        ///     Sound attached to audio source.
        /// </summary>
        public ISound? Sound { get; set; }

        /// <summary>
        ///     Indicates whether this audio source is currently playing a sound.
        /// </summary>
        public bool IsPlaying { get; internal set; }

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);

            if (Sound is null)
            {
                componentDataWriter.WriteNull("Sound");
            }
            else
            {
                componentDataWriter.WriteAssetId("Sound", assetStore.GetAssetId(Sound));
            }

            componentDataWriter.WriteBool("IsPlaying", IsPlaying);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);

            Sound = componentDataReader.IsNull("Sound") ? null : assetStore.GetAsset<ISound>(componentDataReader.ReadAssetId("Sound"));
            IsPlaying = componentDataReader.ReadBool("IsPlaying");
        }
    }

    internal sealed class AudioSourceComponentFactory : ComponentFactory<AudioSourceComponent>
    {
        protected override AudioSourceComponent CreateComponent() => new AudioSourceComponent();
    }
}