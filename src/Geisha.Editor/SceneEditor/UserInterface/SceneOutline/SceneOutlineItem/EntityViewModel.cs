using Geisha.Editor.Core;
using Geisha.Editor.Core.Properties;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor;

namespace Geisha.Editor.SceneEditor.UserInterface.SceneOutline.SceneOutlineItem
{
    internal sealed class EntityViewModel : SceneOutlineItemViewModel
    {
        private readonly EntityModel _entityModel;
        private readonly IEventBus _eventBus;
        private readonly IEntityPropertiesEditorViewModelFactory _entityPropertiesEditorViewModelFactory;

        public EntityViewModel(EntityModel entityModel, IEventBus eventBus, IEntityPropertiesEditorViewModelFactory entityPropertiesEditorViewModelFactory)
        {
            _entityModel = entityModel;
            _eventBus = eventBus;
            _entityPropertiesEditorViewModelFactory = entityPropertiesEditorViewModelFactory;

            Name = _entityModel.Name;

            foreach (var model in _entityModel.Children)
            {
                Items.Add(new EntityViewModel(model, _eventBus, _entityPropertiesEditorViewModelFactory));
            }

            ContextMenuItems.Add(new ContextMenuItem("Add child entity", RelayCommand.Create(AddChildEntity)));

            _entityModel.EntityAdded += EntityModelOnEntityAdded;
            _entityModel.NameChanged += EntityModelOnNameChanged;
        }

        public override void OnSelected()
        {
            var viewModel = _entityPropertiesEditorViewModelFactory.Create(_entityModel);
            _eventBus.SendEvent(new PropertiesSubjectChangedEvent(viewModel));
        }

        private void AddChildEntity()
        {
            _entityModel.AddChildEntity();
        }

        private void EntityModelOnEntityAdded(object? sender, EntityAddedEventArgs e)
        {
            Items.Add(new EntityViewModel(e.EntityModel, _eventBus, _entityPropertiesEditorViewModelFactory));
        }

        private void EntityModelOnNameChanged(object? sender, PropertyChangedEventArgs<string> e)
        {
            Name = e.NewValue;
        }
    }
}