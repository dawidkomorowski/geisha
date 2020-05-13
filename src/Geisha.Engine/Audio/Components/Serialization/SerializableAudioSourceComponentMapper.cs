using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Audio.Components.Serialization
{
    internal sealed class SerializableAudioSourceComponentMapper : SerializableComponentMapperAdapter<AudioSourceComponent, SerializableAudioSourceComponent>
    {
        private readonly IAssetStore _assetStore;

        public SerializableAudioSourceComponentMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SerializableAudioSourceComponent MapToSerializable(AudioSourceComponent component)
        {
            var sound = component.Sound ??
                        throw new InvalidOperationException($"{nameof(AudioSourceComponent)}.{nameof(AudioSourceComponent.Sound)} cannot be null.");

            return new SerializableAudioSourceComponent
            {
                SoundAssetId = _assetStore.GetAssetId(sound).Value,
                IsPlaying = component.IsPlaying
            };
        }

        protected override AudioSourceComponent MapFromSerializable(SerializableAudioSourceComponent serializableComponent)
        {
            return new AudioSourceComponent
            {
                Sound = _assetStore.GetAsset<ISound>(new AssetId(serializableComponent.SoundAssetId)),
                IsPlaying = serializableComponent.IsPlaying
            };
        }
    }
}