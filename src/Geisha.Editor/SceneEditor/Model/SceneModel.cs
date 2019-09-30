using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.Model
{
    public sealed class SceneModel
    {
        private readonly Scene _scene;

        public SceneModel(Scene scene)
        {
            _scene = scene;
        }
    }
}