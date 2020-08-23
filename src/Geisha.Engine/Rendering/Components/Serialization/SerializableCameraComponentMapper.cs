using System;
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
                AspectRatioBehavior = component.AspectRatioBehavior.ToString(),
                ViewRectangle = SerializableVector2.FromVector2(component.ViewRectangle)
            };
        }

        protected override CameraComponent MapFromSerializable(SerializableCameraComponent serializableComponent)
        {
            if (serializableComponent.AspectRatioBehavior == null)
                throw new ArgumentException(
                    $"{nameof(SerializableCameraComponent)}.{nameof(SerializableCameraComponent.AspectRatioBehavior)} cannot be null.");

            return new CameraComponent
            {
                AspectRatioBehavior = Enum.Parse<AspectRatioBehavior>(serializableComponent.AspectRatioBehavior),
                ViewRectangle = SerializableVector2.ToVector2(serializableComponent.ViewRectangle)
            };
        }
    }
}