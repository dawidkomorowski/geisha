using System.ComponentModel.Composition;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.Engine.Rendering.Components.Definition
{
    [Export(typeof(IComponentDefinitionMapper))]
    public class CameraDefinitionMapper : ComponentDefinitionMapperAdapter<Camera, CameraDefinition>
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