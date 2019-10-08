using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SceneEditorViewModel : DocumentContentViewModel
    {
        private readonly IEventBus _eventBus;
        private readonly SceneModel _sceneModel;

        public SceneEditorViewModel(SceneModel sceneModel, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _sceneModel = sceneModel;
        }

        public string SceneInstance => _sceneModel.GetHashCode().ToString();

        public override void OnDocumentSelected()
        {
            _eventBus.SendEvent(new SelectedSceneModelChangedEvent(_sceneModel));
        }
    }
}