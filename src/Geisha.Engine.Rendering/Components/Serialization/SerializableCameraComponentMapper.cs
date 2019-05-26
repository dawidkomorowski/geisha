using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal class SerializableCameraComponentMapper : SerializableComponentMapperAdapter<CameraComponent, SerializableCameraComponent>
    {
        protected override SerializableCameraComponent MapToSerializable(CameraComponent component)
        {
            return new SerializableCameraComponent();
        }

        protected override CameraComponent MapFromSerializable(SerializableCameraComponent serializableComponent)
        {
            return new CameraComponent();
        }
    }
}