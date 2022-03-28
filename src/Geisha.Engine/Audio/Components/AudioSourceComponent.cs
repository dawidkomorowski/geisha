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
        internal AudioSourceComponent(Entity entity) : base(entity)
        {
        }

        /// <summary>
        ///     Sound attached to audio source.
        /// </summary>
        public ISound? Sound { get; set; }

        /// <summary>
        ///     Indicates whether this audio source is currently playing a sound.
        /// </summary>
        public bool IsPlaying { get; internal set; }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);

            if (Sound is null)
            {
                writer.WriteNull("Sound");
            }
            else
            {
                writer.WriteAssetId("Sound", assetStore.GetAssetId(Sound));
            }

            writer.WriteBool("IsPlaying", IsPlaying);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);

            Sound = reader.IsNull("Sound") ? null : assetStore.GetAsset<ISound>(reader.ReadAssetId("Sound"));
            IsPlaying = reader.ReadBool("IsPlaying");
        }
    }

    internal sealed class AudioSourceComponentFactory : ComponentFactory<AudioSourceComponent>
    {
        protected override AudioSourceComponent CreateComponent(Entity entity) => new AudioSourceComponent(entity);
    }
}