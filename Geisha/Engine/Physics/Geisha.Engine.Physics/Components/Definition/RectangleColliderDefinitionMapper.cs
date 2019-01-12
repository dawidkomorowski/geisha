using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Definition
{
    internal class RectangleColliderDefinitionMapper : SerializableComponentMapperAdapter<RectangleCollider, RectangleColliderDefinition>
    {
        protected override RectangleColliderDefinition MapToSerializable(RectangleCollider component)
        {
            return new RectangleColliderDefinition
            {
                Dimension = SerializableVector2.FromVector2(component.Dimension)
            };
        }

        protected override RectangleCollider MapFromSerializable(RectangleColliderDefinition serializableComponent)
        {
            return new RectangleCollider
            {
                Dimension = SerializableVector2.ToVector2(serializableComponent.Dimension)
            };
        }
    }
}