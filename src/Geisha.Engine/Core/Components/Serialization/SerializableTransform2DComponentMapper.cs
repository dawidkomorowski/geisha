using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    internal sealed class SerializableTransform2DComponentMapper : SerializableComponentMapperAdapter<Transform2DComponent, SerializableTransform2DComponent>
    {
        protected override SerializableTransform2DComponent MapToSerializable(Transform2DComponent component)
        {
            return new SerializableTransform2DComponent
            {
                Translation = SerializableVector2.FromVector2(component.Translation),
                Rotation = component.Rotation,
                Scale = SerializableVector2.FromVector2(component.Scale)
            };
        }

        protected override Transform2DComponent MapFromSerializable(SerializableTransform2DComponent serializableComponent)
        {
            return new Transform2DComponent
            {
                Translation = SerializableVector2.ToVector2(serializableComponent.Translation),
                Rotation = serializableComponent.Rotation,
                Scale = SerializableVector2.ToVector2(serializableComponent.Scale)
            };
        }
    }
}