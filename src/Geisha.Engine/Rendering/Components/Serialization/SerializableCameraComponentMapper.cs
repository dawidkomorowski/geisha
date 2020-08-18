using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal class SerializableCameraComponentMapper : SerializableComponentMapperAdapter<CameraComponent, SerializableCameraComponent>
    {
        protected override SerializableCameraComponent MapToSerializable(CameraComponent component)
        {
            return new SerializableCameraComponent
            {
                ViewRectangle = SerializableVector2.FromVector2(component.ViewRectangle)
            };
        }

        protected override CameraComponent MapFromSerializable(SerializableCameraComponent serializableComponent)
        {
            return new CameraComponent
            {
                ViewRectangle = SerializableVector2.ToVector2(serializableComponent.ViewRectangle)
            };
        }
    }
}