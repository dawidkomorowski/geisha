using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent
{
    internal sealed class TransformComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<double> _translationX;
        private readonly IProperty<double> _translationY;
        private readonly IProperty<double> _translationZ;
        private readonly IProperty<double> _rotationX;
        private readonly IProperty<double> _rotationY;
        private readonly IProperty<double> _rotationZ;
        private readonly IProperty<double> _scaleX;
        private readonly IProperty<double> _scaleY;
        private readonly IProperty<double> _scaleZ;

        public TransformComponentPropertiesEditorViewModel(TransformComponentModel componentModel) : base(componentModel)
        {
            _translationX = CreateProperty(nameof(TranslationX), componentModel.TranslationX);
            _translationY = CreateProperty(nameof(TranslationY), componentModel.TranslationY);
            _translationZ = CreateProperty(nameof(TranslationZ), componentModel.TranslationZ);
            _rotationX = CreateProperty(nameof(RotationX), componentModel.RotationX);
            _rotationY = CreateProperty(nameof(RotationY), componentModel.RotationY);
            _rotationZ = CreateProperty(nameof(RotationZ), componentModel.RotationZ);
            _scaleX = CreateProperty(nameof(ScaleX), componentModel.ScaleX);
            _scaleY = CreateProperty(nameof(ScaleY), componentModel.ScaleY);
            _scaleZ = CreateProperty(nameof(ScaleZ), componentModel.ScaleZ);

            _translationX.Subscribe(v => componentModel.TranslationX = v);
            _translationY.Subscribe(v => componentModel.TranslationY = v);
            _translationZ.Subscribe(v => componentModel.TranslationZ = v);
            _rotationX.Subscribe(v => componentModel.RotationX = v);
            _rotationY.Subscribe(v => componentModel.RotationY = v);
            _rotationZ.Subscribe(v => componentModel.RotationZ = v);
            _scaleX.Subscribe(v => componentModel.ScaleX = v);
            _scaleY.Subscribe(v => componentModel.ScaleY = v);
            _scaleZ.Subscribe(v => componentModel.ScaleZ = v);

            View = new TransformComponentPropertiesEditorView(this);
        }

        public override Control View { get; }

        public double TranslationX
        {
            get => _translationX.Get();
            set => _translationX.Set(value);
        }

        public double TranslationY
        {
            get => _translationY.Get();
            set => _translationY.Set(value);
        }

        public double TranslationZ
        {
            get => _translationZ.Get();
            set => _translationZ.Set(value);
        }

        public double RotationX
        {
            get => _rotationX.Get();
            set => _rotationX.Set(value);
        }

        public double RotationY
        {
            get => _rotationY.Get();
            set => _rotationY.Set(value);
        }

        public double RotationZ
        {
            get => _rotationZ.Get();
            set => _rotationZ.Set(value);
        }

        public double ScaleX
        {
            get => _scaleX.Get();
            set => _scaleX.Set(value);
        }

        public double ScaleY
        {
            get => _scaleY.Get();
            set => _scaleY.Set(value);
        }

        public double ScaleZ
        {
            get => _scaleZ.Get();
            set => _scaleZ.Set(value);
        }
    }
}