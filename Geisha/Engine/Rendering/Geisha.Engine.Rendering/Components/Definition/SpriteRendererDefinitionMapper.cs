using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Definition;

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
                OrderInLayer = 2,
                SpriteAssetId = _assetStore.GetAssetId(component.Sprite)
            };
        }

        protected override SpriteRenderer FromDefinition(SpriteRendererDefinition componentDefinition)
        {
            throw new NotImplementedException();
        }
    }
}