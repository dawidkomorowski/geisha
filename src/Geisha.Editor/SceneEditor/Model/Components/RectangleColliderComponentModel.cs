﻿using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    public class RectangleColliderComponentModel : IComponentModel
    {
        private readonly RectangleColliderComponent _component;

        public RectangleColliderComponentModel(RectangleColliderComponent component)
        {
            _component = component;
        }

        public string Name => "Rectangle Collider Component";

        public Vector2 Dimensions
        {
            get => _component.Dimensions;
            set => _component.Dimensions = value;
        }
    }
}