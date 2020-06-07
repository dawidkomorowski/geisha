using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Serialization
{
    internal sealed class SerializableTransform3DComponentMapper : SerializableComponentMapperAdapter<Transform3DComponent, SerializableTransform3DComponent>
    {
        protected override SerializableTransform3DComponent MapToSerializable(Transform3DComponent component)
        {
            return new SerializableTransform3DComponent
            {
                Translation = SerializableVector3.FromVector3(component.Translation),
                Rotation = SerializableVector3.FromVector3(component.Rotation),
                Scale = SerializableVector3.FromVector3(component.Scale)
            };
        }

        protected override Transform3DComponent MapFromSerializable(SerializableTransform3DComponent serializableComponent)
        {
            return new Transform3DComponent
            {
                Translation = SerializableVector3.ToVector3(serializableComponent.Translation),
                Rotation = SerializableVector3.ToVector3(serializableComponent.Rotation),
                Scale = SerializableVector3.ToVector3(serializableComponent.Scale)
            };
        }
    }
}