using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components.Definition
{
    internal class SpriteRendererDefinitionMapper : ComponentDefinitionMapperAdapter<SpriteRenderer, SpriteRendererDefinition>
    {
        private readonly IAssetStore _assetStore;

        public SpriteRendererDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SpriteRendererDefinition ToDefinition(SpriteRenderer component)
        {
            return new SpriteRendererDefinition
            {
                Visible = component.Visible,
                SortingLayerName = component.SortingLayerName,
                OrderInLayer = component.OrderInLayer,
                SpriteAssetId = _assetStore.GetAssetId(component.Sprite)
            };
        }

        protected override SpriteRenderer FromDefinition(SpriteRendererDefinition componentDefinition)
        {
            return new SpriteRenderer
            {
                Visible = componentDefinition.Visible,
                SortingLayerName = componentDefinition.SortingLayerName,
                OrderInLayer = componentDefinition.OrderInLayer,
                Sprite = _assetStore.GetAsset<Sprite>(componentDefinition.SpriteAssetId)
            };
        }
    }
}