using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Serialization
{
    internal class RectangleColliderDefinitionMapper : SerializableComponentMapperAdapter<RectangleColliderComponent, RectangleColliderDefinition>
    {
        protected override RectangleColliderDefinition MapToSerializable(RectangleColliderComponent component)
        {
            return new RectangleColliderDefinition
            {
                Dimension = SerializableVector2.FromVector2(component.Dimension)
            };
        }

        protected override RectangleColliderComponent MapFromSerializable(RectangleColliderDefinition serializableComponent)
        {
            return new RectangleColliderComponent
            {
                Dimension = SerializableVector2.ToVector2(serializableComponent.Dimension)
            };
        }
    }
}