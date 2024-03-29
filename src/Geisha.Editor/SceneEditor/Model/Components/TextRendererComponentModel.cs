﻿using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    internal sealed class TextRendererComponentModel : IComponentModel
    {
        private readonly TextRendererComponent _component;

        public TextRendererComponentModel(TextRendererComponent component)
        {
            _component = component;
        }

        public string Name => "Text Renderer Component";

        public string Text
        {
            get => _component.Text;
            set => _component.Text = value;
        }

        public string FontFamilyName
        {
            get => _component.FontFamilyName;
            set => _component.FontFamilyName = value;
        }

        public FontSize FontSize
        {
            get => _component.FontSize;
            set => _component.FontSize = value;
        }

        public Color Color
        {
            get => _component.Color;
            set => _component.Color = value;
        }

        public double MaxWidth
        {
            get => _component.MaxWidth;
            set => _component.MaxWidth = value;
        }

        public double MaxHeight
        {
            get => _component.MaxHeight;
            set => _component.MaxHeight = value;
        }

        public TextAlignment TextAlignment
        {
            get => _component.TextAlignment;
            set => _component.TextAlignment = value;
        }

        public ParagraphAlignment ParagraphAlignment
        {
            get => _component.ParagraphAlignment;
            set => _component.ParagraphAlignment = value;
        }

        public Vector2 Pivot
        {
            get => _component.Pivot;
            set => _component.Pivot = value;
        }

        public bool ClipToLayoutBox
        {
            get => _component.ClipToLayoutBox;
            set => _component.ClipToLayoutBox = value;
        }

        public bool Visible
        {
            get => _component.Visible;
            set => _component.Visible = value;
        }

        public string SortingLayerName
        {
            get => _component.SortingLayerName;
            set => _component.SortingLayerName = value;
        }

        public int OrderInLayer
        {
            get => _component.OrderInLayer;
            set => _component.OrderInLayer = value;
        }
    }
}