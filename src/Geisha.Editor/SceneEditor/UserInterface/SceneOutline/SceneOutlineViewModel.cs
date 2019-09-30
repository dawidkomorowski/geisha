using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline
{
    internal sealed class SceneOutlineViewModel : ViewModel
    {
        private readonly IEventBus _eventBus;
        private readonly IProperty<string> _sceneInstance;
        private SceneModel _sceneModel;

        public SceneOutlineViewModel(IEventBus eventBus)
        {
            _eventBus = eventBus;

            _sceneInstance = CreateProperty<string>(nameof(SceneInstance));

            _eventBus.RegisterEventHandler<SelectedSceneModelChangedEvent>(SelectedSceneModelChangedEventHandler);
        }

        public string SceneInstance
        {
            get => _sceneInstance.Get();
            set => _sceneInstance.Set(value);
        }

        private void SelectedSceneModelChangedEventHandler(SelectedSceneModelChangedEvent @event)
        {
            _sceneModel = @event.SceneModel;
            SceneInstance = _sceneModel?.GetHashCode().ToString();
        }
    }
}