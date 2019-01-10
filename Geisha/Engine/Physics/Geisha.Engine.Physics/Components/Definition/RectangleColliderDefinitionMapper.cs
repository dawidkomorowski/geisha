using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Physics.Components.Definition
{
    internal class RectangleColliderDefinitionMapper : ComponentDefinitionMapperAdapter<RectangleCollider, RectangleColliderDefinition>
    {
        protected override RectangleColliderDefinition ToDefinition(RectangleCollider component)
        {
            return new RectangleColliderDefinition
            {
                Dimension = SerializableVector2.FromVector2(component.Dimension)
            };
        }

        protected override RectangleCollider FromDefinition(RectangleColliderDefinition componentDefinition)
        {
            return new RectangleCollider
            {
                Dimension = SerializableVector2.ToVector2(componentDefinition.Dimension)
            };
        }
    }
}