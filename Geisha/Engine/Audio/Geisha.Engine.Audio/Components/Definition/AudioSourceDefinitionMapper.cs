using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components.Definition
{
    internal sealed class AudioSourceDefinitionMapper : ComponentDefinitionMapperAdapter<AudioSource, AudioSourceDefinition>
    {
        private readonly IAssetStore _assetStore;

        public AudioSourceDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override AudioSourceDefinition ToDefinition(AudioSource component)
        {
            return new AudioSourceDefinition
            {
                SoundAssetId = _assetStore.GetAssetId(component.Sound),
                IsPlaying = component.IsPlaying
            };
        }

        protected override AudioSource FromDefinition(AudioSourceDefinition componentDefinition)
        {
            return new AudioSource
            {
                Sound = _assetStore.GetAsset<ISound>(componentDefinition.SoundAssetId),
                IsPlaying = componentDefinition.IsPlaying
            };
        }
    }
}