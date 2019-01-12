using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components.Definition
{
    internal class CameraDefinitionMapper : SerializableComponentMapperAdapter<Camera, CameraDefinition>
    {
        protected override CameraDefinition MapToSerializable(Camera component)
        {
            return new CameraDefinition();
        }

        protected override Camera MapFromSerializable(CameraDefinition serializableComponent)
        {
            return new Camera();
        }
    }
}