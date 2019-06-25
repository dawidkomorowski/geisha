using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Mapping;

namespace Geisha.Engine.Input.Components.Serialization
{
    internal class SerializableInputComponentMapper : SerializableComponentMapperAdapter<InputComponent, SerializableInputComponent>
    {
        private readonly IAssetStore _assetStore;

        public SerializableInputComponentMapper(IAssetStore assetStore)
        {
            _assetStore = assetStore;
        }

        protected override SerializableInputComponent MapToSerializable(InputComponent component)
        {
            return new SerializableInputComponent
            {
                InputMappingAssetId = component.InputMapping != null ? _assetStore.GetAssetId(component.InputMapping).Value : (Guid?) null
            };
        }

        protected override InputComponent MapFromSerializable(SerializableInputComponent serializableComponent)
        {
            return new InputComponent
            {
                InputMapping = serializableComponent.InputMappingAssetId != null
                    ? _assetStore.GetAsset<InputMapping>(new AssetId(serializableComponent.InputMappingAssetId.Value))
                    : null,
                HardwareInput = HardwareInput.Empty
            };
        }
    }
}