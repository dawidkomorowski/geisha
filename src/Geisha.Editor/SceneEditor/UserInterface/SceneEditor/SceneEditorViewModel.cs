using System;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SceneEditorViewModel : DocumentContentViewModel
    {
        private readonly IEventBus _eventBus;
        private readonly SceneModel _sceneModel;
        private readonly Action _saveSceneAction;

        public SceneEditorViewModel(SceneModel sceneModel, IEventBus eventBus, Action saveSceneAction)
        {
            _sceneModel = sceneModel;
            _eventBus = eventBus;
            _saveSceneAction = saveSceneAction;
        }

        public string SceneInstance => _sceneModel.GetHashCode().ToString();

        public override void OnDocumentSelected()
        {
            _eventBus.SendEvent(new SelectedSceneModelChangedEvent(_sceneModel));
        }

        public override void SaveDocument()
        {
            _saveSceneAction();
        }
    }
}