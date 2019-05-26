using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Serialization
{
    internal class SerializableRectangleColliderComponentMapper : SerializableComponentMapperAdapter<RectangleColliderComponent,
        SerializableRectangleColliderComponent>
    {
        protected override SerializableRectangleColliderComponent MapToSerializable(RectangleColliderComponent component)
        {
            return new SerializableRectangleColliderComponent
            {
                Dimension = SerializableVector2.FromVector2(component.Dimension)
            };
        }

        protected override RectangleColliderComponent MapFromSerializable(SerializableRectangleColliderComponent serializableComponent)
        {
            return new RectangleColliderComponent
            {
                Dimension = SerializableVector2.ToVector2(serializableComponent.Dimension)
            };
        }
    }
}