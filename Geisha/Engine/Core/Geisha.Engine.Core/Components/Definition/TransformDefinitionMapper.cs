using Geisha.Common.Math.Definition;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Core.Components.Definition
{
    internal class TransformDefinitionMapper : ComponentDefinitionMapperAdapter<Transform, TransformDefinition>
    {
        protected override TransformDefinition ToDefinition(Transform component)
        {
            return new TransformDefinition
            {
                Translation = Vector3Definition.FromVector3(component.Translation),
                Rotation = Vector3Definition.FromVector3(component.Rotation),
                Scale = Vector3Definition.FromVector3(component.Scale)
            };
        }

        protected override Transform FromDefinition(TransformDefinition componentDefinition)
        {
            return new Transform
            {
                Translation = Vector3Definition.ToVector3(componentDefinition.Translation),
                Rotation = Vector3Definition.ToVector3(componentDefinition.Rotation),
                Scale = Vector3Definition.ToVector3(componentDefinition.Scale)
            };
        }
    }
}