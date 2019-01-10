using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components.Definition
{
    internal class CircleColliderDefinitionMapper : ComponentDefinitionMapperAdapter<CircleCollider, CircleColliderDefinition>
    {
        protected override CircleColliderDefinition ToDefinition(CircleCollider component)
        {
            return new CircleColliderDefinition
            {
                Radius = component.Radius
            };
        }

        protected override CircleCollider FromDefinition(CircleColliderDefinition componentDefinition)
        {
            return new CircleCollider
            {
                Radius = componentDefinition.Radius
            };
        }
    }
}