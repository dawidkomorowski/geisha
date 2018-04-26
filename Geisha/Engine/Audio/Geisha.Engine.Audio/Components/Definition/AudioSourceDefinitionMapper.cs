using System.ComponentModel.Composition;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components.Definition
{
    [Export(typeof(IComponentDefinitionMapper))]
    internal sealed class AudioSourceDefinitionMapper : ComponentDefinitionMapperAdapter<AudioSource, AudioSourceDefinition>
    {
        private readonly IAssetStore _assetStore;

        [ImportingConstructor]
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