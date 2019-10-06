using System.Collections.ObjectModel;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline
{
    internal sealed class SceneOutlineViewModel : ViewModel
    {
        private readonly IEventBus _eventBus;
        private SceneModel _sceneModel;

        public SceneOutlineViewModel(IEventBus eventBus)
        {
            _eventBus = eventBus;

            _eventBus.RegisterEventHandler<SelectedSceneModelChangedEvent>(SelectedSceneModelChangedEventHandler);
        }

        public ObservableCollection<SceneOutlineItemViewModel> Items { get; } = new ObservableCollection<SceneOutlineItemViewModel>();

        private void SelectedSceneModelChangedEventHandler(SelectedSceneModelChangedEvent @event)
        {
            _sceneModel = @event.SceneModel;
            Items.Clear();
            Items.Add(new SceneRootViewModel(_sceneModel));
        }
    }
}