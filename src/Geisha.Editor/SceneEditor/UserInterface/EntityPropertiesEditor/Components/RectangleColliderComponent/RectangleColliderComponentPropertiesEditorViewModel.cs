using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    internal sealed class RectangleColliderComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<double> _dimensionX;
        private readonly IProperty<double> _dimensionY;

        public RectangleColliderComponentPropertiesEditorViewModel(RectangleColliderComponentModel componentModel) : base(componentModel)
        {
            _dimensionX = CreateProperty(nameof(DimensionX), componentModel.DimensionX);
            _dimensionY = CreateProperty(nameof(DimensionY), componentModel.DimensionY);

            _dimensionX.Subscribe(v => componentModel.DimensionX = v);
            _dimensionY.Subscribe(v => componentModel.DimensionY = v);

            View = new RectangleColliderComponentPropertiesEditorView(this);
        }

        public override Control View { get; }

        public double DimensionX
        {
            get => _dimensionX.Get();
            set => _dimensionX.Set(value);
        }

        public double DimensionY
        {
            get => _dimensionY.Get();
            set => _dimensionY.Set(value);
        }
    }
}