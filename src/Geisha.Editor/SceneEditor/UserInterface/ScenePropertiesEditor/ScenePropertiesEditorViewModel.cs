using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor
{
    internal sealed class ScenePropertiesEditorViewModel : ViewModel
    {
        private readonly SceneModel _sceneModel;
        private readonly IProperty<string> _constructionScript;

        public ScenePropertiesEditorViewModel(SceneModel sceneModel)
        {
            _sceneModel = sceneModel;
            _constructionScript = CreateProperty(nameof(ConstructionScript), _sceneModel.ConstructionScript);
            _constructionScript.Subscribe(script => _sceneModel.ConstructionScript = script);
        }

        public string ConstructionScript
        {
            get => _constructionScript.Get();
            set => _constructionScript.Set(value);
        }
    }
}