using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components
{
    public abstract class ComponentPropertiesEditorViewModel : ViewModel
    {
        protected ComponentPropertiesEditorViewModel(IComponentModel componentModel)
        {
            Name = componentModel.Name;
        }

        public string Name { get; }

        // TODO Is it good idea to define view here? Or maybe it should be parent view that resolved correct view for given view model?
        // TODO ViewFactory similar to ComponentViewModelFactory?
        public abstract Control View { get; }
    }
}