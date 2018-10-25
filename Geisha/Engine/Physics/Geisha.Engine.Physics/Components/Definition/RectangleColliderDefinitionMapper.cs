using Geisha.Common.Math.Definition;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Physics.Components.Definition
{
    internal class RectangleColliderDefinitionMapper : ComponentDefinitionMapperAdapter<RectangleCollider, RectangleColliderDefinition>
    {
        protected override RectangleColliderDefinition ToDefinition(RectangleCollider component)
        {
            return new RectangleColliderDefinition
            {
                Dimension = Vector2Definition.FromVector2(component.Dimension)
            };
        }

        protected override RectangleCollider FromDefinition(RectangleColliderDefinition componentDefinition)
        {
            return new RectangleCollider
            {
                Dimension = Vector2Definition.ToVector2(componentDefinition.Dimension)
            };
        }
    }
}