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

        public EntityViewModel(EntityModel entityModel, IEventBus eventBus)
        {
            _entityModel = entityModel;
            _eventBus = eventBus;

            Name = _entityModel.Name;

            foreach (var model in _entityModel.Children)
            {
                Items.Add(new EntityViewModel(model, _eventBus));
            }

            ContextMenuItems.Add(new ContextMenuItem("Add child entity", new RelayCommand(AddChildEntity)));

            _entityModel.EntityAdded += EntityModelOnEntityAdded;
            _entityModel.NameChanged += EntityModelOnNameChanged;
        }

        public override void OnSelected()
        {
            var viewModel = new EntityPropertiesEditorViewModel(_entityModel);
            var view = new EntityPropertiesEditorView(viewModel);
            _eventBus.SendEvent(new PropertiesSubjectChangedEvent(view));
        }

        private void AddChildEntity()
        {
            _entityModel.AddChildEntity();
        }

        private void EntityModelOnEntityAdded(object sender, EntityAddedEventArgs e)
        {
            Items.Add(new EntityViewModel(e.EntityModel, _eventBus));
        }

        private void EntityModelOnNameChanged(object sender, PropertyChangedEventArgs<string> e)
        {
            Name = e.NewValue;
        }
    }
}