using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal class SerializableSpriteRendererComponentMapper : SerializableComponentMapperAdapter<SpriteRendererComponent, SerializableSpriteRendererComponent>
    {
        private readonly IAssetStore _assetStore;

        public SerializableSpriteRendererComponentMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SerializableSpriteRendererComponent MapToSerializable(SpriteRendererComponent component)
        {
            return new SerializableSpriteRendererComponent
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                SpriteAssetId = _assetStore.GetAssetId(component.Sprite)
            };
        }

        protected override SpriteRendererComponent MapFromSerializable(SerializableSpriteRendererComponent serializableComponent)
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