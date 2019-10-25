using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor
{
    internal interface IEntityPropertiesEditorViewModelFactory
    {
        EntityPropertiesEditorViewModel Create(EntityModel entityModel);
    }

    internal sealed class EntityPropertiesEditorViewModelFactory : IEntityPropertiesEditorViewModelFactory
    {
        private readonly IComponentPropertiesEditorViewModelFactory _componentPropertiesEditorViewModelFactory;

        public EntityPropertiesEditorViewModelFactory(IComponentPropertiesEditorViewModelFactory componentPropertiesEditorViewModelFactory)
        {
            _componentPropertiesEditorViewModelFactory = componentPropertiesEditorViewModelFactory;
        }

        public EntityPropertiesEditorViewModel Create(EntityModel entityModel)
        {
            return new EntityPropertiesEditorViewModel(entityModel, _componentPropertiesEditorViewModelFactory);
        }
    }
}