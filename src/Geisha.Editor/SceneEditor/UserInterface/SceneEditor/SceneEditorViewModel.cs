using System;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneEditor
{
    internal sealed class SceneEditorViewModel : DocumentContentViewModel
    {
        private readonly string _sceneFilePath;
        private readonly IEventBus _eventBus;
        private readonly ISceneLoader _sceneLoader;
        private readonly Scene _scene;
        private readonly SceneModel _sceneModel;

        public SceneEditorViewModel(string sceneFilePath, IEventBus eventBus, ISceneLoader sceneLoader)
        {
            _sceneFilePath = sceneFilePath;
            _eventBus = eventBus;
            _sceneLoader = sceneLoader;

            _scene = _sceneLoader.Load(_sceneFilePath);

            throw new NotImplementedException();
            // TODO _sceneModel = new SceneModel(_scene);
        }

        public string SceneInstance => _sceneModel.GetHashCode().ToString();

        public override void OnDocumentSelected()
        {
            _eventBus.SendEvent(new SelectedSceneModelChangedEvent(_sceneModel));
        }

        public override void SaveDocument()
        {
            _sceneLoader.Save(_scene, _sceneFilePath);
        }
    }
}