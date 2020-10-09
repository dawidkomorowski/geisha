using System;
using System.Diagnostics;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal sealed class SerializableSpriteRendererComponentMapper
        : SerializableComponentMapperAdapter<SpriteRendererComponent, SerializableSpriteRendererComponent>
    {
        private readonly IAssetStore _assetStore;

        public SerializableSpriteRendererComponentMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SerializableSpriteRendererComponent MapToSerializable(SpriteRendererComponent component)
        {
            Debug.Assert(component.Sprite != null,
                "component.Sprite != null"); // TODO If it is ok to have null Sprite on component it should be supported in serialization. Maybe it will be fixed in #192.

            return new SerializableSpriteRendererComponent
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                SpriteAssetId = _assetStore.GetAssetId(component.Sprite).Value
            };
        }

        protected override SpriteRendererComponent MapFromSerializable(SerializableSpriteRendererComponent serializableComponent)
        {
            if (serializableComponent.SortingLayerName == null)
                throw new ArgumentException(
                    $"{nameof(SerializableSpriteRendererComponent)}.{nameof(SerializableSpriteRendererComponent.SortingLayerName)} cannot be null.");

            return new SpriteRendererComponent
            {
                Visible = serializableComponent.Visible,
                SortingLayerName = serializableComponent.SortingLayerName,
                OrderInLayer = serializableComponent.OrderInLayer,
                Sprite = _assetStore.GetAsset<Sprite>(new AssetId(serializableComponent.SpriteAssetId))
            };
        }
    }
}