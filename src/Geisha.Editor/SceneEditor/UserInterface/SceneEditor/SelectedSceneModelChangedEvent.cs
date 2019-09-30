using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SelectedSceneModelChangedEvent : IEvent
    {
        public SelectedSceneModelChangedEvent(SceneModel sceneModel)
        {
            SceneModel = sceneModel;
        }

        public SceneModel SceneModel { get; }
    }
}