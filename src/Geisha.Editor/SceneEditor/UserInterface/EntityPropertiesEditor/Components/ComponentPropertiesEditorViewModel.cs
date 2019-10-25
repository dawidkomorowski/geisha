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
        public abstract Control View { get; }
    }
}