﻿using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    public sealed class Transform3DComponentModel : IComponentModel
    {
        private readonly Transform3DComponent _component;

        public Transform3DComponentModel(Transform3DComponent component)
        {
            _component = component;
        }

        public string Name => "Transform 3D Component";

        public Vector3 Translation
        {
            get => _component.Translation;
            set => _component.Translation = value;
        }

        public Vector3 Rotation
        {
            get => _component.Rotation;
            set => _component.Rotation = value;
        }

        public Vector3 Scale
        {
            get => _component.Scale;
            set => _component.Scale = value;
        }
    }
}