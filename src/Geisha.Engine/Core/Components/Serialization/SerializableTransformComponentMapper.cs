using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    internal sealed class SerializableTransformComponentMapper : SerializableComponentMapperAdapter<TransformComponent, SerializableTransformComponent>
    {
        protected override SerializableTransformComponent MapToSerializable(TransformComponent component)
        {
            return new SerializableTransformComponent
            {
                Translation = SerializableVector3.FromVector3(component.Translation),
                Rotation = SerializableVector3.FromVector3(component.Rotation),
                Scale = SerializableVector3.FromVector3(component.Scale)
            };
        }

        protected override TransformComponent MapFromSerializable(SerializableTransformComponent serializableComponent)
        {
            return new TransformComponent
            {
                Translation = SerializableVector3.ToVector3(serializableComponent.Translation),
                Rotation = SerializableVector3.ToVector3(serializableComponent.Rotation),
                Scale = SerializableVector3.ToVector3(serializableComponent.Scale)
            };
        }
    }
}