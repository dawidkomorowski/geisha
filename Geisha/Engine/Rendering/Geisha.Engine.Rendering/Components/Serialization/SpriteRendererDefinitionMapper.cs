using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal class SpriteRendererDefinitionMapper : SerializableComponentMapperAdapter<SpriteRendererComponent, SpriteRendererDefinition>
    {
        private readonly IAssetStore _assetStore;

        public SpriteRendererDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SpriteRendererDefinition MapToSerializable(SpriteRendererComponent component)
        {
            return new SpriteRendererDefinition
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                SpriteAssetId = _assetStore.GetAssetId(component.Sprite)
            };
        }

        protected override SpriteRendererComponent MapFromSerializable(SpriteRendererDefinition serializableComponent)
        {
            return new SpriteRendererComponent
            {
                Visible = serializableComponent.Visible,
                SortingLayerName = serializableComponent.SortingLayerName,
                OrderInLayer = serializableComponent.OrderInLayer,
                Sprite = _assetStore.GetAsset<Sprite>(serializableComponent.SpriteAssetId)
            };
        }
    }
}