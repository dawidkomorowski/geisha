using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Components.Definition
{
    internal class InputComponentDefinitionMapper : SerializableComponentMapperAdapter<InputComponent, InputComponentDefinition>
    {
        private readonly IAssetStore _assetStore;

        public InputComponentDefinitionMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override InputComponentDefinition MapToSerializable(InputComponent component)
        {
            return new InputComponentDefinition
            {
                InputMappingAssetId = component.InputMapping != null ? _assetStore.GetAssetId(component.InputMapping) : (Guid?) null
            };
        }

        protected override InputComponent MapFromSerializable(InputComponentDefinition serializableComponent)
        {
            return new InputComponent
            {
                InputMapping = serializableComponent.InputMappingAssetId != null
                    ? _assetStore.GetAsset<InputMapping>(serializableComponent.InputMappingAssetId.Value)
                    : null,
                HardwareInput = HardwareInput.Empty
            };
        }
    }
}