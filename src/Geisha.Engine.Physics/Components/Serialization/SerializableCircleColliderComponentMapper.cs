using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Serialization
{
    internal class SerializableCircleColliderComponentMapper : SerializableComponentMapperAdapter<CircleColliderComponent, SerializableCircleColliderComponent>
    {
        protected override SerializableCircleColliderComponent MapToSerializable(CircleColliderComponent component)
        {
            return new SerializableCircleColliderComponent
            {
                Radius = component.Radius
            };
        }

        protected override CircleColliderComponent MapFromSerializable(SerializableCircleColliderComponent serializableComponent)
        {
            return new CircleColliderComponent
            {
                Radius = serializableComponent.Radius
            };
        }
    }
}