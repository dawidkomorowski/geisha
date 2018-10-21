using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Rendering.Components.Definition
{
    internal class CameraDefinitionMapper : ComponentDefinitionMapperAdapter<Camera, CameraDefinition>
    {
        protected override CameraDefinition ToDefinition(Camera component)
        {
            return new CameraDefinition();
        }

        protected override Camera FromDefinition(CameraDefinition componentDefinition)
        {
            return new Camera();
        }
    }
}