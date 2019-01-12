using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components.Definition
{
    internal sealed class AudioSourceDefinitionMapper : SerializableComponentMapperAdapter<AudioSource, AudioSourceDefinition>
    {
        private readonly IAssetStore _assetStore;

        public AudioSourceDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override AudioSourceDefinition MapToSerializable(AudioSource component)
        {
            return new AudioSourceDefinition
            {
                SoundAssetId = _assetStore.GetAssetId(component.Sound),
                IsPlaying = component.IsPlaying
            };
        }

        protected override AudioSource MapFromSerializable(AudioSourceDefinition serializableComponent)
        {
            return new AudioSource
            {
                Sound = _assetStore.GetAsset<ISound>(serializableComponent.SoundAssetId),
                IsPlaying = serializableComponent.IsPlaying
            };
        }
    }
}