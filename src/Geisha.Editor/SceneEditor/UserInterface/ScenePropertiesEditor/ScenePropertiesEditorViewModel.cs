using System.Collections.Generic;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor
{
    internal sealed class ScenePropertiesEditorViewModel : ViewModel
    {
        private readonly SceneModel _sceneModel;
        private readonly IProperty<SceneBehaviorName> _sceneBehavior;

        public ScenePropertiesEditorViewModel(SceneModel sceneModel)
        {
            _sceneModel = sceneModel;
            _sceneBehavior = CreateProperty(nameof(SceneBehavior), _sceneModel.SceneBehavior);
            _sceneBehavior.Subscribe(sceneBehavior => _sceneModel.SceneBehavior = sceneBehavior);

            AvailableSceneBehaviors = _sceneModel.AvailableSceneBehaviors;
        }

        public IEnumerable<SceneBehaviorName> AvailableSceneBehaviors { get; }

        public SceneBehaviorName SceneBehavior
        {
            get => _sceneBehavior.Get();
            set => _sceneBehavior.Set(value);
        }
    }
}