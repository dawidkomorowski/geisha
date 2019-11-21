using Geisha.Editor.Core;
using Geisha.Editor.Core.Properties;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;
using Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal sealed class SceneRootViewModel : SceneOutlineItemViewModel
    {
        private readonly SceneModel _sceneModel;
        private readonly IEventBus _eventBus;
        private readonly IEntityPropertiesEditorViewModelFactory _entityPropertiesEditorViewModelFactory;

        public SceneRootViewModel(SceneModel sceneModel, IEventBus eventBus, IEntityPropertiesEditorViewModelFactory entityPropertiesEditorViewModelFactory)
        {
            _sceneModel = sceneModel;
            _eventBus = eventBus;
            _entityPropertiesEditorViewModelFactory = entityPropertiesEditorViewModelFactory;

            Name = "Scene";

            foreach (var entityModel in _sceneModel.RootEntities)
            {
                Items.Add(new EntityViewModel(entityModel, _eventBus, _entityPropertiesEditorViewModelFactory));
            }

            ContextMenuItems.Add(new ContextMenuItem("Add entity", new RelayCommand(AddEntity)));

            _sceneModel.EntityAdded += SceneModelOnEntityAdded;
        }

        public override void OnSelected()
        {
            var viewModel = new ScenePropertiesEditorViewModel(_sceneModel);
            _eventBus.SendEvent(new PropertiesSubjectChangedEvent(viewModel));
        }

        private void AddEntity()
        {
            _sceneModel.AddEntity();
        }

        private void SceneModelOnEntityAdded(object sender, EntityAddedEventArgs e)
        {
            Items.Add(new EntityViewModel(e.EntityModel, _eventBus, _entityPropertiesEditorViewModelFactory));
        }
    }
}