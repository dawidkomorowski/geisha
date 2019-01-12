using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Definition
{
    internal class SpriteRendererDefinitionMapper : SerializableComponentMapperAdapter<SpriteRenderer, SpriteRendererDefinition>
    {
        private readonly IAssetStore _assetStore;

        public SpriteRendererDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SpriteRendererDefinition MapToSerializable(SpriteRenderer component)
        {
            return new SpriteRendererDefinition
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                SpriteAssetId = _assetStore.GetAssetId(component.Sprite)
            };
        }

        protected override SpriteRenderer MapFromSerializable(SpriteRendererDefinition serializableComponent)
        {
            return new SpriteRenderer
            {
                Visible = serializableComponent.Visible,
                SortingLayerName = serializableComponent.SortingLayerName,
                OrderInLayer = serializableComponent.OrderInLayer,
                Sprite = _assetStore.GetAsset<Sprite>(serializableComponent.SpriteAssetId)
            };
        }
    }
}