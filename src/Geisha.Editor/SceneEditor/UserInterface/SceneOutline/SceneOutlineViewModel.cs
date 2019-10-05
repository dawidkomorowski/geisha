using System.Collections.ObjectModel;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem;
using Geisha.Engine.Core.SceneModel;

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

            _sceneModel = CreateTestSceneModel();
            Items.Add(new SceneRootViewModel(_sceneModel));
        }

        public ObservableCollection<SceneOutlineItemViewModel> Items { get; } = new ObservableCollection<SceneOutlineItemViewModel>();

        private void SelectedSceneModelChangedEventHandler(SelectedSceneModelChangedEvent @event)
        {
            _sceneModel = @event.SceneModel;
            Items.Clear();
            Items.Add(new SceneRootViewModel(_sceneModel));
        }

        private SceneModel CreateTestSceneModel()
        {
            var scene = new Scene();

            var entity1 = new Entity {Name = "Entity 1"};
            var entity11 = new Entity {Name = "Entity 1.1", Parent = entity1};
            scene.AddEntity(entity1);

            var entity2 = new Entity {Name = "Entity 2"};
            scene.AddEntity(entity2);

            var entity3 = new Entity {Name = "Entity 3"};
            scene.AddEntity(entity3);

            return new SceneModel(scene);
        }
    }
}