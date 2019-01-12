using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Definition
{
    internal class CircleColliderDefinitionMapper : SerializableComponentMapperAdapter<CircleCollider, CircleColliderDefinition>
    {
        protected override CircleColliderDefinition MapToSerializable(CircleCollider component)
        {
            return new CircleColliderDefinition
            {
                Radius = component.Radius
            };
        }

        protected override CircleCollider MapFromSerializable(CircleColliderDefinition serializableComponent)
        {
            return new CircleCollider
            {
                Radius = serializableComponent.Radius
            };
        }
    }
}