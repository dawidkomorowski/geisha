using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent
{
    internal sealed class CircleColliderComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<double> _radius;

        public CircleColliderComponentPropertiesEditorViewModel(CircleColliderComponentModel componentModel) : base(componentModel)
        {
            _radius = CreateProperty(nameof(Radius), componentModel.Radius);
            _radius.Subscribe(v => componentModel.Radius = v);
        }

        public double Radius
        {
            get => _radius.Get();
            set => _radius.Set(value);
        }
    }
}