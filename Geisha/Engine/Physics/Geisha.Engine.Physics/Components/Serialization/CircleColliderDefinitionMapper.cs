using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Serialization
{
    internal class CircleColliderDefinitionMapper : SerializableComponentMapperAdapter<CircleColliderComponent, CircleColliderDefinition>
    {
        protected override CircleColliderDefinition MapToSerializable(CircleColliderComponent component)
        {
            return new CircleColliderDefinition
            {
                Radius = component.Radius
            };
        }

        protected override CircleColliderComponent MapFromSerializable(CircleColliderDefinition serializableComponent)
        {
            return new CircleColliderComponent
            {
                Radius = serializableComponent.Radius
            };
        }
    }
}