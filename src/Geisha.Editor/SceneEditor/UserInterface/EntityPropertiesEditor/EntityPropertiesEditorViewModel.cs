using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor
{
    internal sealed class EntityPropertiesEditorViewModel : ViewModel
    {
        private readonly EntityModel _entityModel;
        private readonly IProperty<string> _name;

        public EntityPropertiesEditorViewModel(EntityModel entityModel)
        {
            _entityModel = entityModel;
            _name = CreateProperty(nameof(Name), _entityModel.Name);
        }

        public string Name
        {
            get => _name.Get();
            set => _name.Set(value);
        }
    }
}