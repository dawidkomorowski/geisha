using System;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor
{
    internal sealed class ScenePropertiesEditorViewModel : ViewModel
    {
        private readonly SceneModel _sceneModel;
        private readonly IProperty<string> _sceneBehaviorName;

        public ScenePropertiesEditorViewModel(SceneModel sceneModel)
        {
            _sceneModel = sceneModel;
            _sceneBehaviorName = CreateProperty(nameof(SceneBehaviorName), _sceneModel.SceneBehavior.Value);
            _sceneBehaviorName.Subscribe(behaviorName => _sceneModel.SceneBehavior = new SceneBehaviorName(behaviorName));
        }

        public string SceneBehaviorName
        {
            get => _sceneBehaviorName.Get();
            set => _sceneBehaviorName.Set(value);
        }
    }
}