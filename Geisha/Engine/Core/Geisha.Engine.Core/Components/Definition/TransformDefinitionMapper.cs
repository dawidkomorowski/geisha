using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Core.Components.Definition
{
    internal sealed class TransformDefinitionMapper : ComponentDefinitionMapperAdapter<Transform, TransformDefinition>
    {
        protected override TransformDefinition ToDefinition(Transform component)
        {
            return new TransformDefinition
            {
                Translation = SerializableVector3.FromVector3(component.Translation),
                Rotation = SerializableVector3.FromVector3(component.Rotation),
                Scale = SerializableVector3.FromVector3(component.Scale)
            };
        }

        protected override Transform FromDefinition(TransformDefinition componentDefinition)
        {
            return new Transform
            {
                Translation = SerializableVector3.ToVector3(componentDefinition.Translation),
                Rotation = SerializableVector3.ToVector3(componentDefinition.Rotation),
                Scale = SerializableVector3.ToVector3(componentDefinition.Scale)
            };
        }
    }
}