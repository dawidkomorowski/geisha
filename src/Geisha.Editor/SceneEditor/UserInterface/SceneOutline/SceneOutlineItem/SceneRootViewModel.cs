using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal sealed class SceneRootViewModel : SceneOutlineItemViewModel
    {
        private readonly SceneModel _sceneModel;
        private readonly IEventBus _eventBus;

        public SceneRootViewModel(SceneModel sceneModel, IEventBus eventBus)
        {
            _sceneModel = sceneModel;
            _eventBus = eventBus;

            Name = "Scene";

            foreach (var entityModel in _sceneModel.RootEntities)
            {
                Items.Add(new EntityViewModel(entityModel, _eventBus));
            }

            ContextMenuItems.Add(new ContextMenuItem("Add entity", new RelayCommand(AddEntity)));

            _sceneModel.EntityAdded += SceneModelOnEntityAdded;
        }

        private void AddEntity()
        {
            _sceneModel.AddEntity();
        }

        private void SceneModelOnEntityAdded(object sender, EntityAddedEventArgs e)
        {
            Items.Add(new EntityViewModel(e.EntityModel, _eventBus));
        }

        public override void OnSelected()
        {
        }
    }
}