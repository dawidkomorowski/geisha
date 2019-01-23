using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Serialization
{
    internal class CameraDefinitionMapper : SerializableComponentMapperAdapter<CameraComponent, CameraDefinition>
    {
        protected override CameraDefinition MapToSerializable(CameraComponent component)
        {
            return new CameraDefinition();
        }

        protected override CameraComponent MapFromSerializable(CameraDefinition serializableComponent)
        {
            return new CameraComponent();
        }
    }
}