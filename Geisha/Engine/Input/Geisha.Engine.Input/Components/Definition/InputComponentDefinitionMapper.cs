using System;
using System.ComponentModel.Composition;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Components.Definition
{
    [Export(typeof(IComponentDefinitionMapper))]
    internal class InputComponentDefinitionMapper : ComponentDefinitionMapperAdapter<InputComponent, InputComponentDefinition>
    {
        private readonly IAssetStore _assetStore;

        [ImportingConstructor]
        public InputComponentDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override InputComponentDefinition ToDefinition(InputComponent component)
        {
            return new InputComponentDefinition
            {
                InputMappingAssetId = component.InputMapping != null ? _assetStore.GetAssetId(component.InputMapping) : (Guid?) null
            };
        }

        protected override InputComponent FromDefinition(InputComponentDefinition componentDefinition)
        {
            return new InputComponent
            {
                InputMapping = componentDefinition.InputMappingAssetId != null
                    ? _assetStore.GetAsset<InputMapping>(componentDefinition.InputMappingAssetId.Value)
                    : null,
                HardwareInput = HardwareInput.Empty
            };
        }
    }
}