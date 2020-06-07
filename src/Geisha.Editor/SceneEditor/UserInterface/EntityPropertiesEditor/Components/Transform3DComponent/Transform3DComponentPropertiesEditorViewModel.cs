using Geisha.Common.Math;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.Transform3DComponent
{
    internal sealed class Transform3DComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<Vector3> _translation;
        private readonly IProperty<Vector3> _rotation;
        private readonly IProperty<Vector3> _scale;

        public Transform3DComponentPropertiesEditorViewModel(Transform3DComponentModel componentModel) : base(componentModel)
        {
            _translation = CreateProperty(nameof(Translation), componentModel.Translation);
            _rotation = CreateProperty(nameof(Rotation), componentModel.Rotation);
            _scale = CreateProperty(nameof(Scale), componentModel.Scale);

            _translation.Subscribe(v => componentModel.Translation = v);
            _rotation.Subscribe(v => componentModel.Rotation = v);
            _scale.Subscribe(v => componentModel.Scale = v);
        }

        public Vector3 Translation
        {
            get => _translation.Get();
            set => _translation.Set(value);
        }

        public Vector3 Rotation
        {
            get => _rotation.Get();
            set => _rotation.Set(value);
        }

        public Vector3 Scale
        {
            get => _scale.Get();
            set => _scale.Set(value);
        }
    }
}