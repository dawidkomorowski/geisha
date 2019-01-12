using Geisha.Common.Math.Serialization;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Core.Components.Definition
{
    internal sealed class TransformDefinitionMapper : SerializableComponentMapperAdapter<Transform, TransformDefinition>
    {
        protected override TransformDefinition MapToSerializable(Transform component)
        {
            return new TransformDefinition
            {
                Translation = SerializableVector3.FromVector3(component.Translation),
                Rotation = SerializableVector3.FromVector3(component.Rotation),
                Scale = SerializableVector3.FromVector3(component.Scale)
            };
        }

        protected override Transform MapFromSerializable(TransformDefinition serializableComponent)
        {
            return new Transform
            {
                Translation = SerializableVector3.ToVector3(serializableComponent.Translation),
                Rotation = SerializableVector3.ToVector3(serializableComponent.Rotation),
                Scale = SerializableVector3.ToVector3(serializableComponent.Scale)
            };
        }
    }
}