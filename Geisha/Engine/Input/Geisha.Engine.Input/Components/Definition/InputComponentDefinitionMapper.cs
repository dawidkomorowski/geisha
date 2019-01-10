using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Components.Definition
{
    internal class InputComponentDefinitionMapper : ComponentDefinitionMapperAdapter<InputComponent, InputComponentDefinition>
    {
        private readonly IAssetStore _assetStore;

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